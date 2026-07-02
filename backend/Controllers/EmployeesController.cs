using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;
using backend.Filters;
using backend.Models;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[MgAuthorize]
public class EmployeesController(AppDbContext db) : ControllerBase
{
    private string UserMainCode => (HttpContext.Items["auth_user"] as HrEmp)?.MainCode ?? "";

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var employees = await db.HrEmps
            .Where(e => e.MainCode == UserMainCode)
            .Select(e => new
            {
                id         = e.EmpNo,
                firstName  = (e.FullNameTh ?? e.FullNameEn ?? "").Split(' ').FirstOrDefault() ?? "",
                lastName   = string.Join(" ", (e.FullNameTh ?? e.FullNameEn ?? "").Split(' ').Skip(1)),
                fullName   = e.FullNameTh ?? e.FullNameEn,
                email      = e.Email,
                phone      = e.Phone,
                position   = e.EmpPos,
                dptCode    = e.DptCode,
                status     = e.EmpStatus ?? "active",
                startDate  = e.WorkStartDate,
            })
            .ToListAsync();

        return Ok(employees);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var e = await db.HrEmps.FirstOrDefaultAsync(x => x.EmpNo == id && x.MainCode == UserMainCode);
        if (e is null) return NotFound();
        return Ok(new
        {
            id        = e.EmpNo,
            fullName  = e.FullNameTh ?? e.FullNameEn,
            email     = e.Email,
            phone     = e.Phone,
            position  = e.EmpPos,
            dptCode   = e.DptCode,
            status    = e.EmpStatus,
            startDate = e.WorkStartDate,
        });
    }
}
