using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BaseController : ControllerBase
{
    // [Route("/api/whoami")]
    // public IActionResult WhoAmI() {
    //     return Ok(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    // }
}