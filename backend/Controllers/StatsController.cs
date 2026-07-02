using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Filters;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[MgAuthorize]
public class StatsController(AppDbContext db) : ControllerBase
{
    private string UserMainCode => (HttpContext.Items["auth_user"] as HrEmp)?.MainCode ?? "";

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var mainCode = UserMainCode;
        var emps = await db.HrEmps.Where(e => e.MainCode == mainCode).ToListAsync();
        var divs = await db.HrDivisions.Where(d => d.MainCode == mainCode).ToListAsync();

        var total      = emps.Count;
        var active     = emps.Count(e => e.EmpStatus != "X" && e.EmpStatus != "0");
        var deptCount  = divs.Count;

        var recentHires = emps
            .Where(e => e.WorkStartDate.HasValue)
            .OrderByDescending(e => e.WorkStartDate)
            .Take(5)
            .Select(e => new
            {
                id       = e.EmpNo,
                fullName = e.FullNameTh ?? e.FullNameEn,
                position = e.EmpPos,
                status   = e.EmpStatus ?? "active"
            });

        var depts = divs.Select(d => new
        {
            id   = d.DivCode,
            name = d.DivName ?? d.DivNameEng,
            _count = new { employees = emps.Count(e => e.DptCode == d.DivCode) }
        });

        return Ok(new
        {
            total,
            active,
            onLeave   = 0,
            deptCount,
            avgSalary = 0,
            recentHires,
            depts
        });
    }
}
