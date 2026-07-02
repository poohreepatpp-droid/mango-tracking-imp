using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Data.SqlClient;
using FastMember;
using System.Collections.Concurrent;
using System.Data;
using backend.Models;

namespace backend.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<HrEmp>      HrEmps      => Set<HrEmp>();
    public DbSet<HrDivision> HrDivisions => Set<HrDivision>();
    public DbSet<AppAuthen>  AppAuthens  => Set<AppAuthen>();
    public DbSet<Maincomp>   Maincomps   => Set<Maincomp>();

    public IDbContextTransaction? CurrentTransactionObject { get; set; }

    // Thread-safe Random with per-thread seed (avoid contention in concurrent retry)
    private static readonly ThreadLocal<Random> _threadRandom =
        new(() => new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId)));
    private static Random Rng => _threadRandom.Value!;

    // Async error-log queue (max 5000 items; drops to ThreadPool on overflow)
    private record ErrorLogItem(string Path, string Filename, string Message);
    private static readonly BlockingCollection<ErrorLogItem> _logQueue =
        new(new ConcurrentQueue<ErrorLogItem>(), 5000);
    private static readonly Lazy<Task> _logWorker =
        new(() => Task.Factory.StartNew(DrainLogQueue,
            CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default));

    // Throttled cleanup: prune log files older than 7 days, at most once per hour per path
    private static readonly ConcurrentDictionary<string, DateTime> _cleanupTimes = new(StringComparer.OrdinalIgnoreCase);

    #region EF Core configuration

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<HrEmp>()
            .HasKey(e => new { e.EmpNo, e.MainCode });

        modelBuilder.Entity<HrDivision>()
            .HasKey(d => new { d.MainCode, d.DivCode });

        modelBuilder.Entity<AppAuthen>()
            .HasKey(a => a.SessionId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer(KeyFileReader.ConnectionString + ";Encrypt=Yes;TrustServerCertificate=Yes;");
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<decimal>().HavePrecision(18, 10);
        configurationBuilder.Properties<decimal?>().HavePrecision(18, 10);
    }

    #endregion

    #region Helpers

    public string? esc(string? input) => input?.Replace("'", "''");
    public DateTime? GetDate() => DateTime.Now;

    /// <summary>Format DateTime? as SQL literal — NULL when null.</summary>
    public static string SqlDate(DateTime? date)
        => date.HasValue ? $"'{date.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL";

    /// <summary>Build a safe IN (...) clause from a collection. Returns (NULL) when empty.</summary>
    public static string CreateInClause<T>(IEnumerable<T>? items)
    {
        if (items == null || !items.Any()) return "(NULL)";
        var parts = items.Select(item => item switch
        {
            null            => "NULL",
            string s        => $"'{s.Replace("'", "''")}'",
            int or long or double or decimal or float => item.ToString()!,
            DateTime dt     => SqlDate(dt),
            _               => throw new ArgumentException($"Unsupported type {typeof(T).Name}")
        });
        return $"({string.Join(",", parts)})";
    }

    #endregion

    #region Transaction helpers

    private static bool IsDeadlockException(SqlException ex)
        => ex.Number is 1205   // Deadlock victim
                     or 1222   // Lock request timeout
                     or 8645   // Timeout waiting for memory resource
                     or 8651;  // Low memory

    private static bool IsTransientException(SqlException ex)
        => ex.Number is -1 or 2 or 19 or 20 or 53 or 64 or 233
                       or 10053 or 10054 or 10060
                       or 40197 or 40501 or 40613
                       or 49918 or 49919 or 49920;

    private static void RollbackSafely(IDbContextTransaction trn)
    {
        try
        {
            var dbTrn = trn.GetDbTransaction();
            if (dbTrn.Connection != null)
                trn.Rollback();
        }
        catch (Exception ex) { CreateErrorLog(ex); }
    }

    /// <summary>Synchronous transaction with deadlock retry + exponential backoff.</summary>
    public void ExecuteTransaction(
        Action  action,
        ref string? error_message,
        bool view_bug       = false,
        bool show_log       = true,
        int  maxRetries     = 3,
        int  commandTimeout = 480)
    {
        if (view_bug) { action(); return; }

        int      retry     = 0;
        TimeSpan baseDelay = TimeSpan.FromMilliseconds(100);

        while (retry <= maxRetries)
        {
            using var trn = Database.BeginTransaction(IsolationLevel.ReadCommitted);
            try
            {
                CurrentTransactionObject = trn;
                Database.SetCommandTimeout(commandTimeout);
                action();
                trn.Commit();
                return;
            }
            catch (SqlException sqlEx) when (IsDeadlockException(sqlEx))
            {
                RollbackSafely(trn);
                retry++;
                if (retry > maxRetries)
                {
                    error_message = $"Transaction failed after {maxRetries} deadlock retries: {sqlEx.Message}";
                    if (show_log) CreateErrorLog(sqlEx, error_message);
                    break;
                }
                var delay = baseDelay * Math.Pow(2, retry - 1)
                          + TimeSpan.FromMilliseconds(Rng.Next(0, Math.Max(1, (int)(baseDelay.TotalMilliseconds * Math.Pow(2, retry - 1) * 0.1))));
                if (show_log) CreateErrorLog(null, $"Deadlock (attempt {retry}/{maxRetries}), retrying in {delay.TotalMilliseconds:F0}ms…");
                Thread.Sleep(delay);
                continue;
            }
            catch (Exception ex)
            {
                RollbackSafely(trn);
                var root = ex.InnerException?.GetBaseException() ?? ex;
                error_message = root.Message;
                if (show_log) CreateErrorLog(root, ex.ToString());
                break;
            }
            finally { CurrentTransactionObject = null; }
        }
    }

    /// <summary>Async transaction with deadlock retry + exponential backoff.</summary>
    public async Task<string> ExecuteTransactionAsync(
        Func<Task> action,
        bool view_bug       = false,
        int  maxRetries     = 3,
        int  commandTimeout = 480)
    {
        string error = "";
        if (view_bug) { await action(); return error; }

        int      retry     = 0;
        TimeSpan baseDelay = TimeSpan.FromMilliseconds(100);

        while (retry <= maxRetries)
        {
            using var trn = await Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            try
            {
                CurrentTransactionObject = trn;
                Database.SetCommandTimeout(commandTimeout);
                await action();
                await trn.CommitAsync();
                return error;
            }
            catch (SqlException sqlEx) when (IsDeadlockException(sqlEx))
            {
                RollbackSafely(trn);
                retry++;
                if (retry > maxRetries)
                {
                    error = $"Transaction failed after {maxRetries} deadlock retries: {sqlEx.Message}";
                    CreateErrorLog(sqlEx, error);
                    break;
                }
                var delay = baseDelay * Math.Pow(2, retry - 1)
                          + TimeSpan.FromMilliseconds(Rng.Next(0, 100));
                CreateErrorLog(null, $"Deadlock (attempt {retry}/{maxRetries}), retrying in {delay.TotalMilliseconds:F0}ms…");
                await Task.Delay(delay);
                continue;
            }
            catch (Exception ex)
            {
                RollbackSafely(trn);
                var root = ex.InnerException?.GetBaseException() ?? ex;
                error = root.Message;
                CreateErrorLog(root, ex.ToString());
                break;
            }
            finally { CurrentTransactionObject = null; }
        }
        return error;
    }

    /// <summary>Simple try/catch with transient SQL retry (no DB transaction).</summary>
    public static void TryCatchExecute(Action action, ref string? error_message, int maxRetries = 3)
    {
        int retry = 0;
        while (retry <= maxRetries)
        {
            try { action(); return; }
            catch (SqlException sqlEx) when (retry < maxRetries && IsTransientException(sqlEx))
            {
                retry++;
                var delay = (int)(1000 * Math.Pow(2, retry - 1)) + Rng.Next(0, 1000);
                error_message = $"Retry {retry}/{maxRetries}: {sqlEx.Message}";
                CreateErrorLog(new Exception($"SQL Retry {retry}: {sqlEx.Message}", sqlEx));
                Thread.Sleep(delay);
            }
            catch (Exception ex)
            {
                var root = ex.InnerException?.GetBaseException() ?? ex;
                error_message = root.Message;
                CreateErrorLog(root);
                return;
            }
        }
        error_message = $"Operation failed after {maxRetries} retries.";
    }

    /// <summary>Async try/catch wrapper — returns error string.</summary>
    public static async Task<string> TryCatchExecuteAsync(Func<Task> action)
    {
        try { await action(); return ""; }
        catch (Exception ex)
        {
            var root = ex.InnerException?.GetBaseException() ?? ex;
            CreateErrorLog(root, ex.ToString());
            return root.Message;
        }
    }

    /// <summary>Run action under READ UNCOMMITTED (dirty read). Skips if a transaction is already active.</summary>
    public void StartReadUncommitted(Action action)
    {
        var savedTimeout = Database.GetCommandTimeout();
        Database.SetCommandTimeout(180);

        if (CurrentTransactionObject != null)
        {
            try { action(); }
            finally { Database.SetCommandTimeout(savedTimeout); }
            return;
        }

        using var trn = Database.BeginTransaction(IsolationLevel.ReadUncommitted);
        var savedTrn = CurrentTransactionObject;
        try
        {
            CurrentTransactionObject = trn;
            action();
            trn.Commit();
        }
        catch
        {
            RollbackSafely(trn);
            throw;
        }
        finally
        {
            CurrentTransactionObject = savedTrn;
            Database.SetCommandTimeout(savedTimeout);
        }
    }

    #endregion

    #region Error logging (async queue, throttled cleanup)

    public static void CreateErrorLog(Exception? ex = null, string? message = null)
    {
        var text = message ?? ex?.ToString();
        if (string.IsNullOrWhiteSpace(text)) return;
        try
        {
            var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            TryScheduleCleanup(logDir);
            QueueWrite(logDir, $"error_{DateTime.Now:yyyyMMdd}.log", $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {text}\n\n");
        }
        catch { }
    }

    private static void QueueWrite(string dir, string file, string text)
    {
        _ = _logWorker.Value; // ensure worker started
        var item = new ErrorLogItem(dir, file, text);
        if (!_logQueue.TryAdd(item))
            ThreadPool.QueueUserWorkItem(_ => WriteLog(item));
    }

    private static void DrainLogQueue()
    {
        foreach (var item in _logQueue.GetConsumingEnumerable())
            WriteLog(item);
    }

    private static void WriteLog(ErrorLogItem item)
    {
        try
        {
            Directory.CreateDirectory(item.Path);
            File.AppendAllText(Path.Combine(item.Path, item.Filename), item.Message);
        }
        catch { }
    }

    private static void TryScheduleCleanup(string logDir)
    {
        var now = DateTime.UtcNow;
        var last = _cleanupTimes.GetOrAdd(logDir, DateTime.MinValue);
        if (now - last < TimeSpan.FromHours(1)) return;
        if (!_cleanupTimes.TryUpdate(logDir, now, last)) return;

        Task.Run(() =>
        {
            try
            {
                var cutoff = DateTime.Now.AddDays(-7);
                foreach (var f in new DirectoryInfo(logDir).EnumerateFiles())
                    try { if (f.CreationTime < cutoff) f.Delete(); } catch { }
            }
            catch { }
        });
    }

    #endregion

    #region Raw SQL helpers

    public int SqlInvoke(string sql, params object[] param)
        => Database.ExecuteSqlRaw(sql, param);

    public void FastInsert(string table, object m)
    {
        var props  = m.GetType().GetProperties()
                      .Where(x => !string.IsNullOrEmpty(x.GetValue(m)?.ToString()))
                      .ToList();
        string fields = string.Join(",", props.Select(x => x.Name));
        string marks  = string.Join(",", props.Select((_, i) => $"{{{i}}}"));
        SqlInvoke($"INSERT INTO {table} ({fields}) VALUES ({marks})",
                  props.Select(x => x.GetValue(m)!).ToArray());
    }

    public List<T> SqlFetch<T>(string sql, params object[] param) where T : class, new()
    {
        var ls   = new List<T>();
        var conn = Database.GetDbConnection();
        try { conn.Open(); } catch { }

        using var comm = conn.CreateCommand();
        if (CurrentTransactionObject != null)
            comm.Transaction = CurrentTransactionObject.GetDbTransaction();

        comm.CommandText = sql;
        int ii = 0;
        foreach (var x in param ?? [])
        {
            var p = comm.CreateParameter();
            p.ParameterName = "@p" + ii++;
            p.Value = x ?? DBNull.Value;
            comm.Parameters.Add(p);
        }

        if (typeof(T) == typeof(Dictionary<string, object>))
        {
            using var reader  = comm.ExecuteReader();
            var       columns = new List<string>();
            while (reader.Read())
            {
                if (columns.Count == 0)
                    columns = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
                var d = new Dictionary<string, object>();
                for (int i = 0; i < columns.Count; i++)
                    d[columns[i]] = reader.GetValue(i);
                (ls as List<Dictionary<string, object>>)!.Add(d);
            }
        }
        else
        {
            var accessor = TypeAccessor.Create(typeof(T));
            var members  = accessor.GetMembers().Select(m => m.Name)
                                   .ToHashSet(StringComparer.OrdinalIgnoreCase);
            using var reader = comm.ExecuteReader();
            while (reader.Read())
            {
                var t = new T();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (!reader.IsDBNull(i))
                    {
                        var col = reader.GetName(i);
                        if (members.Contains(col)) accessor[t, col] = reader.GetValue(i);
                    }
                }
                ls.Add(t);
            }
        }

        return ls;
    }

    #endregion
}
