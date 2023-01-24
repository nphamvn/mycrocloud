using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MockServer.WebMVC.Controllers;

[Route("[controller]")]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;

    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    [HttpGet("login")]
    public async Task<IActionResult> Login()
    {
        return Challenge(new AuthenticationProperties
        {
            RedirectUri = "/projects"
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        return SignOut("Cookies", "oidc");
    }

    [Authorize]
    [HttpGet("user")]
    public IActionResult GetUser()
    {
        var user = HttpContext.User;
        return Ok(JsonSerializer.Serialize(user));
    }
}
