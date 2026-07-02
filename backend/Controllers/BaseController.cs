using Microsoft.AspNetCore.Mvc;
using backend.Models;

namespace backend.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    protected HrEmp? AuthUser    => HttpContext.Items["auth_user"]    as HrEmp;
    protected AppAuthen? AuthSession => HttpContext.Items["auth_session"] as AppAuthen;

    protected IActionResult JsonResp(object? data, string? error = null) =>
        Ok(new { success = string.IsNullOrEmpty(error), error, data });

    protected IActionResult JsonError(string error) =>
        Ok(new { success = false, error, data = (object?)null });
}
