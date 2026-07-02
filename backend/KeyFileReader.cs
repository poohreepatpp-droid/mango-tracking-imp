using MangoCryptProvider;
using System.Text.Json;

namespace backend;

public static class KeyFileReader
{
    public static string ConnectionString { get; private set; } = string.Empty;
    public static string DbProvider { get; private set; } = "SQL";

    public static void Read(string keyFilePath)
    {
        var keyData = File.ReadAllText(keyFilePath);
        var tkn = new EncDec();
        tkn.CheckToken(keyData, out var decoded);

        var info = JsonSerializer.Deserialize<LicenseReader>(decoded,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("Failed to parse key file.");

        // Append TrustServerCertificate if not already present (required for .NET 8+)
        var cs = info.connection_string.TrimEnd(';');
        if (!cs.Contains("TrustServerCertificate", StringComparison.OrdinalIgnoreCase))
            cs += ";TrustServerCertificate=True";

        ConnectionString = cs;
        DbProvider = string.IsNullOrEmpty(info.db_provider) ? "SQL" : info.db_provider.ToUpper();
    }

    private class LicenseReader
    {
        public string secret_key { get; set; } = "";
        public string connection_string { get; set; } = "";
        public int? max_user { get; set; } = 0;
        public string db_provider { get; set; } = "";
        public string company { get; set; } = "";
    }

    private class EncDec : Token
    {
        public EncDec() : base("I_LOVE_MANGO") { }
    }
}
