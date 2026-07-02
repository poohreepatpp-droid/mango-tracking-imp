using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Filters;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[MgAuthorize]
public class UserController(AppDbContext db) : ControllerBase
{
    [HttpGet("get_current_user_data")]
    public async Task<IActionResult> GetCurrentUserData()
    {
        var emp     = HttpContext.Items["auth_user"]    as HrEmp;
        var session = HttpContext.Items["auth_session"] as AppAuthen;
        if (emp is null) return Unauthorized();

        var company = await db.Maincomps
            .Where(c => c.MainCode == emp.MainCode)
            .Select(c => new { c.MainCode, c.MainName })
            .FirstOrDefaultAsync();

        var division = await db.HrDivisions
            .Where(d => d.MainCode == emp.MainCode && d.DivCode == emp.DptCode)
            .Select(d => new { d.DivCode, d.DivName, d.DivNameEng })
            .FirstOrDefaultAsync();

        return Ok(new
        {
            success = true,
            error   = (string?)null,
            data    = new
            {
                empno            = emp.EmpNo,
                empcode          = emp.EmpCode,
                userid           = emp.UserId,
                maincode         = emp.MainCode,
                mainname         = company?.MainName,
                is_authenticated = true,
                is_authen        = true,
                is_super_user    = emp.EntreLevel == 4,
                is_admin         = emp.EntreLevel == 1,
                is_admin_it      = emp.EntreLevel == 2,
                session_id       = session?.SessionId,
                empname          = emp.FullNameTh ?? emp.FullNameEn
                                   ?? $"{emp.PreNameTh}{emp.EmpNameTh}".Trim(),
                emppos           = emp.EmpPos,
                emptel           = emp.Telephone ?? emp.Phone,
                empmail          = emp.Email,
                pre_event        = emp.PreEvent,
                dpt_code         = emp.DptCode,
                dpt_name         = division?.DivName,
                dpt_name_eng     = division?.DivNameEng,
                sale             = emp.ActiveSale ?? "N",
                lang_web         = emp.LangWeb ?? "TH",
                approve_ic_req   = emp.MaSignIc,
                last_access      = session?.LastAccess,
                session_expire   = session?.EndDate,
                level            = emp.Level,
            }
        });
    }
}
