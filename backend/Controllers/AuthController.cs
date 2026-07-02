using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using MangoCryptProvider;
using backend.Data;
using backend.Filters;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext db, IConfiguration config) : ControllerBase
{
    private string DefaultMainCode => config["MAINCODE"] ?? Environment.GetEnvironmentVariable("MAINCODE") ?? "";
    private string KeyPwd   => config["MANGO_KEY_PWD"] ?? Environment.GetEnvironmentVariable("MANGO_KEY_PWD") ?? "M@ngo0##$$!!2468##";

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.UserId) || string.IsNullOrWhiteSpace(req.UserPass))
            return BadRequest(new { error = "Username and password are required." });

        var mainCode = string.IsNullOrWhiteSpace(req.MainCode) ? DefaultMainCode : req.MainCode;
        var userIdUpper = req.UserId.ToUpper();

        // No allow_login filter — match Booking project behaviour
        var emp = await db.HrEmps.FirstOrDefaultAsync(e =>
            e.UserId != null && e.UserId.ToUpper() == userIdUpper && e.MainCode == mainCode);

        if (emp is null)
            return Unauthorized(new { success = false, error = "Invalid username or password." });

        // Try decrypt (encode_userpass='Y' path); fall back to plaintext (encode_userpass='N')
        var (decryptedPass, _) = await DecryptPasswordAsync(emp.UserPass);
        var passOk = decryptedPass is not null
            ? decryptedPass.Trim() == req.UserPass.Trim()
            : emp.UserPass?.Trim() == req.UserPass.Trim();

        if (!passOk)
            return Unauthorized(new { success = false, error = "Invalid username or password." });

        var sessionHours = double.TryParse(
            config["SESSION_HOURS"] ?? Environment.GetEnvironmentVariable("SESSION_HOURS"),
            out var sh) ? sh : 1.0;

        var now       = DateTime.Now;
        var sessionId = Guid.NewGuid().ToString("N");
        var token     = new Token("I_LOVE_MANGO").CreateTokenHex(sessionId);

        db.AppAuthens.Add(new AppAuthen
        {
            SessionId  = sessionId,
            EmpNo      = emp.EmpNo,
            MainCode   = mainCode,
            AppName    = "TEMPLATE",
            Platform   = "WEB",
            StartDate  = now,
            EndDate    = now.AddHours(sessionHours),
            LastAccess = now
        });
        await db.SaveChangesAsync();

        return Ok(new
        {
            success = true,
            data    = token,
            user    = new { emp.EmpNo, emp.UserId, fullName = emp.FullNameTh ?? emp.FullNameEn, emp.Level }
        });
    }

    [HttpPost("logout")]
    [MgAuthorize]
    public async Task<IActionResult> Logout()
    {
        Request.Headers.TryGetValue("X-Mango-Auth", out var tokenValues);
        new Token("I_LOVE_MANGO").CheckTokenHex(tokenValues.FirstOrDefault() ?? "", out var sessionId);

        var session = await db.AppAuthens.FirstOrDefaultAsync(a => a.SessionId == sessionId);
        if (session is not null) { db.AppAuthens.Remove(session); await db.SaveChangesAsync(); }

        return Ok(new { success = true });
    }

    [HttpGet("me")]
    [MgAuthorize]
    public IActionResult Me()
    {
        var emp = HttpContext.Items["auth_user"] as HrEmp;
        if (emp is null) return Unauthorized();
        return Ok(new { emp.EmpNo, emp.UserId, fullName = emp.FullNameTh ?? emp.FullNameEn, emp.Level });
    }

    private async Task<(string? password, string? error)> DecryptPasswordAsync(string? encryptedPass)
    {
        if (string.IsNullOrEmpty(encryptedPass)) return (null, "userpass is null/empty");
        try
        {
            await using var conn = new SqlConnection(KeyFileReader.ConnectionString);
            await conn.OpenAsync();
            await using var cmd = conn.CreateCommand();
            // userpass stored as hex varchar (style 2) — NOT base64
            cmd.CommandText = $"""
                OPEN SYMMETRIC KEY [key_pwd2] DECRYPTION BY PASSWORD = N'{KeyPwd}';
                SELECT CONVERT(varchar(500), DECRYPTBYKEY(CONVERT(varbinary(max), @pass, 2)));
                CLOSE SYMMETRIC KEY [key_pwd2];
                """;
            cmd.Parameters.AddWithValue("@pass", encryptedPass);
            var result = await cmd.ExecuteScalarAsync();
            return (result as string, null);
        }
        catch (Exception ex) { return (null, ex.Message); }
    }
}

public class LoginRequest
{
    public string  UserId   { get; set; } = string.Empty;
    public string  UserPass { get; set; } = string.Empty;
    public string? MainCode { get; set; }
}
