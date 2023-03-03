using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace MockServer.Web.Controllers;

[Route("[controller]")]
public class AccountController : Controller
{
    public const string Name = "Account";
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
            RedirectUri = "/"
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        return SignOut("Cookies", "oidc");
    }
}
