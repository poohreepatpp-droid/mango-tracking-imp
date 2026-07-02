namespace backend.Function;

public static class GF
{
    // ── String helpers ────────────────────────────────────────────

    public static string left(string? s, int start, int len)
    {
        if (string.IsNullOrEmpty(s)) return "";
        len = len - start > s.Length ? s.Length : len;
        return s.Substring(start, Math.Max(0, Math.Min(len, s.Length - start)));
    }

    public static string left(string? s, int len)
        => left(s, 0, len);

    public static string right(string? s, int len)
        => left(s, GF.len(s) - len, len);

    public static string trim(string? s)
        => s?.Trim() ?? "";

    public static string mid(string? s, int? start, int? len)
    {
        if (string.IsNullOrEmpty(s)) return "";
        int st = start ?? 0;
        int ln = len   ?? 0;
        if (st >= s.Length) return "";
        ln = Math.Min(ln, s.Length - st);
        return s.Substring(st, Math.Max(0, ln));
    }

    public static int len(string? s)
        => s?.Length ?? 0;

    public static int pos(string? s, string finder)
        => (s?.IndexOf(finder, StringComparison.InvariantCultureIgnoreCase) ?? -1) + 1;

    // ── Type conversion ───────────────────────────────────────────

    public static string _string<T>(T? x)
        => x?.ToString() ?? "";

    public static string _string<T>(T? x, string format)
    {
        if (_string(x) == "") return "";

        var type = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        if (type == typeof(int) || type == typeof(decimal) || type == typeof(double) || type == typeof(float))
            return Convert.ToDecimal(x).ToString(format);

        if (type == typeof(DateTime))
            return Convert.ToDateTime(x).ToString(format);

        return x?.ToString() ?? "";
    }

    public static int _int<T>(T? x)
    {
        int.TryParse(x?.ToString() ?? "", out var val);
        return val;
    }

    public static decimal _decimal<T>(T? x)
    {
        decimal.TryParse(x?.ToString() ?? "", out var val);
        return val;
    }

    public static bool isnull<T>(T? x)
        => string.IsNullOrEmpty(x?.ToString());

    // ── Date ──────────────────────────────────────────────────────

    public static DateTime? f_get_datenow()
        => DateTime.Now;

    public static DateTime? f_parse_date(string? s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        return DateTime.TryParse(s, out var d) ? d : null;
    }

    // ── Random ────────────────────────────────────────────────────

    public static string RandomRunnoID(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
    }

    // ── File size ─────────────────────────────────────────────────

    private static readonly string[] _sizeSuffixes = new[] { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

    public static string SizeSuffix(long value, int decimalPlaces = 2)
    {
        if (decimalPlaces < 0) throw new ArgumentOutOfRangeException(nameof(decimalPlaces));
        if (value < 0)  return "-" + SizeSuffix(-value, decimalPlaces);
        if (value == 0) return string.Format("{0:n" + decimalPlaces + "} bytes", 0);

        int     mag          = (int)Math.Log(value, 1024);
        decimal adjustedSize = (decimal)value / (1L << (mag * 10));

        if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
        {
            mag++;
            adjustedSize /= 1024;
        }

        return string.Format("{0:n" + decimalPlaces + "} {1}", adjustedSize, _sizeSuffixes[mag]);
    }
}
