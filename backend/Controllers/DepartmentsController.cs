using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Filters;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[MgAuthorize]
public class DepartmentsController(AppDbContext db) : ControllerBase
{
    private string UserMainCode => (HttpContext.Items["auth_user"] as HrEmp)?.MainCode ?? "";

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var mainCode = UserMainCode;

        var divisions = await db.HrDivisions
            .Where(d => d.MainCode == mainCode)
            .ToListAsync();

        var empCounts = await db.HrEmps
            .Where(e => e.MainCode == mainCode && e.DptCode != null)
            .GroupBy(e => e.DptCode!)
            .Select(g => new { dptCode = g.Key, count = g.Count() })
            .ToListAsync();

        var result = divisions.Select(d => new
        {
            id            = d.DivCode,
            name          = d.DivName ?? d.DivNameEng,
            nameEng       = d.DivNameEng,
            employeeCount = empCounts.FirstOrDefault(e => e.dptCode == d.DivCode)?.count ?? 0
        });

        return Ok(result);
    }
}
