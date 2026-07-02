using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Data;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublicController(AppDbContext db) : ControllerBase
{
    [HttpGet("login-companies")]
    public async Task<IActionResult> LoginCompanies()
    {
        var companies = await db.Maincomps
            .Where(c => c.Show == "Y")
            .OrderBy(c => c.DefaultLogin)
            .ThenBy(c => c.MainCode)
            .Select(c => new
            {
                mainCode     = c.MainCode,
                mainName     = c.MainName,
                webRe        = c.WebRe,
                defaultLogin = c.DefaultLogin
            })
            .ToListAsync();

        return Ok(new { success = true, data = companies });
    }
}
