using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using backend.Data;
using backend.Filters;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/sql-update")]
[MgAuthorize]
public class SqlUpdateController(AppDbContext db, IWebHostEnvironment env) : BaseController
{
    private string SqlFolder => Path.Combine(env.ContentRootPath, "UpdateStructure");

    // EntreLevel 4 = super_user, 1 = admin
    private bool IsAuthorized => AuthUser?.EntreLevel is 1 or 4;

    // ── List .sql files ───────────────────────────────────────────────────
    [HttpGet]
    public IActionResult GetFiles()
    {
        if (!IsAuthorized) return JsonError("Insufficient permissions");
        var dir = new DirectoryInfo(SqlFolder);
        if (!dir.Exists) dir.Create();

        var files = dir.GetFiles("*.sql")
                       .OrderBy(f => f.Name)
                       .Select(f => new
                       {
                           filename = f.Name,
                           size     = f.Length,
                           modified = f.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                       });
        return JsonResp(files);
    }

    // ── Get file content (for preview) ────────────────────────────────────
    [HttpGet("content/{filename}")]
    public async Task<IActionResult> GetContent(string filename)
    {
        if (!IsAuthorized) return JsonError("Insufficient permissions");
        if (!IsValidFilename(filename)) return JsonError("Invalid filename");

        var path = Path.Combine(SqlFolder, filename);
        if (!System.IO.File.Exists(path)) return NotFound();

        var content = await System.IO.File.ReadAllTextAsync(path);
        return JsonResp(new { filename, content });
    }

    // ── Execute a SQL file ────────────────────────────────────────────────
    [HttpPost("run/{filename}")]
    public async Task<IActionResult> RunFile(string filename)
    {
        if (!IsAuthorized) return JsonError("Insufficient permissions");
        if (!IsValidFilename(filename)) return JsonError("Invalid filename");

        var path = Path.Combine(SqlFolder, filename);
        if (!System.IO.File.Exists(path)) return NotFound();

        var content = await System.IO.File.ReadAllTextAsync(path);
        var batches = SplitBatches(content);
        var results = new List<BatchResult>();

        foreach (var batch in batches)
        {
            var sql = batch.Trim();
            if (string.IsNullOrWhiteSpace(sql)) continue;
            try
            {
                await db.Database.ExecuteSqlRawAsync(sql);
                results.Add(new BatchResult(TruncateSql(sql), "ok", null));
            }
            catch (Exception ex)
            {
                var root = ex.InnerException?.Message ?? ex.Message;
                results.Add(new BatchResult(TruncateSql(sql), "error", root));
                AppDbContext.CreateErrorLog(ex, $"SqlUpdate [{filename}]: {root}");
            }
        }

        var allOk = results.All(r => r.Status == "ok");
        return JsonResp(new { filename, all_ok = allOk, results });
    }

    // ── Upload a .sql file ────────────────────────────────────────────────
    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (!IsAuthorized) return JsonError("Insufficient permissions");
        if (file is null || file.Length == 0) return JsonError("No file provided");
        if (!file.FileName.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
            return JsonError("Only .sql files are allowed");
        if (!IsValidFilename(file.FileName)) return JsonError("Invalid filename");

        var dir = new DirectoryInfo(SqlFolder);
        if (!dir.Exists) dir.Create();

        var path = Path.Combine(SqlFolder, Path.GetFileName(file.FileName));
        using var stream = System.IO.File.Create(path);
        await file.CopyToAsync(stream);

        return JsonResp(new { filename = file.FileName, size = file.Length });
    }

    // ─────────────────────────────────────────────────────────────────────
    private record BatchResult(string Sql, string Status, string? Message);

    private static bool IsValidFilename(string f)
        => f.EndsWith(".sql", StringComparison.OrdinalIgnoreCase)
           && !f.Contains('/')
           && !f.Contains('\\')
           && !f.Contains("..");

    private static List<string> SplitBatches(string sql)
    {
        // 1. SSMS batch separator: GO on its own line
        var byGo = Regex.Split(sql, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        if (byGo.Length > 1)
            return byGo.Where(b => !string.IsNullOrWhiteSpace(b)).ToList();

        // 2. Split by semicolons — handles single-line statements (SELECT, ALTER TABLE, etc.)
        //    Multi-line blocks without trailing ; (e.g. CREATE TABLE ending with ));) stay as one chunk
        var bySemi = sql.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (bySemi.Length > 1)
            return bySemi.Where(b => !string.IsNullOrWhiteSpace(b)).ToList();

        return [sql.Trim()];
    }

    private static string TruncateSql(string sql)
    {
        // Show first non-empty line as the "command" label (like Booking's data.command)
        var first = sql.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                       .Select(l => l.Trim())
                       .FirstOrDefault(l => l.Length > 0) ?? "";
        return first.Length > 200 ? first[..200] + "…" : first;
    }
}
