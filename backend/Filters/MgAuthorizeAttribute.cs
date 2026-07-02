using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MangoCryptProvider;
using backend.Data;
using backend.Models;

namespace backend.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class MgAuthorizeAttribute : Attribute, IAuthorizationFilter
{
    // Throttle: delete expired sessions at most every 90 seconds (in-process, mirrors Booking)
    private static DateTime? _lastCleanup;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        context.HttpContext.Request.Headers.TryGetValue("X-Mango-Auth", out var tokenValues);
        var token = tokenValues.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(token))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Unauthorized" });
            return;
        }

        new Token("I_LOVE_MANGO").CheckTokenHex(token, out var sessionId);
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Invalid token" });
            return;
        }

        var db  = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
        var cfg = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

        // Throttled cleanup: delete expired rows every 90 seconds
        var now = DateTime.Now;
        if (_lastCleanup == null || _lastCleanup.Value < now)
        {
            try
            {
                db.AppAuthens.RemoveRange(db.AppAuthens.Where(a => a.EndDate < now));
                db.SaveChanges();
            }
            catch { }
            _lastCleanup = now.AddSeconds(90);
        }

        var session = db.AppAuthens.FirstOrDefault(a =>
            a.SessionId == sessionId &&
            a.AppName   == "TEMPLATE" &&
            a.EndDate   >  now);

        if (session is null)
        {
            context.Result = new UnauthorizedObjectResult(new { error = "Session expired" });
            return;
        }

        var emp = db.HrEmps.FirstOrDefault(e =>
            e.EmpNo == session.EmpNo && e.MainCode == session.MainCode);

        if (emp is null)
        {
            context.Result = new UnauthorizedObjectResult(new { error = "User not found" });
            return;
        }

        // Renew session: update last_access + extend end_date (mirrors Booking's MG_TIME logic)
        var mgTimeStr = cfg["SESSION_HOURS"] ?? Environment.GetEnvironmentVariable("SESSION_HOURS");
        var mgTime    = double.TryParse(mgTimeStr, out var h) ? h : 1.0; // default 1 hour
        session.LastAccess = now;
        session.EndDate    = now.AddHours(mgTime);
        db.SaveChanges();

        context.HttpContext.Items["auth_user"]    = emp;
        context.HttpContext.Items["session_id"]   = sessionId;
        context.HttpContext.Items["auth_session"] = session;
    }
}
