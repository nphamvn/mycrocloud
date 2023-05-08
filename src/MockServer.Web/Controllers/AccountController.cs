using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MockServer.Web.Models.Account;
using MockServer.Web.Services;

namespace MockServer.Web.Controllers;

//[Authorize]
[Route("[controller]")]
public class AccountController : Controller
{
    public const string Name = "Account";
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IAuthService _authService;

    public AccountController(ILogger<AccountController> logger
        ,UserManager<IdentityUser> userManager
        , IAuthService authService
        )
    {
        _logger = logger;
        _userManager = userManager;
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpGet("login")]
    public async Task<IActionResult> Login(string returnUrl)
    {
        //var redirectUrl = Url.Action("Auth0Callback", "Account" , new { ReturnUrl = returnUrl });
        // return Challenge(new AuthenticationProperties
        // {
        //     RedirectUri = "/"
        // });
        return View("Views/Account/Login.cshtml");
    }

    [HttpPost("local-login")]
    public async Task<IActionResult> LocalLogin(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return View("Views/Account/Login.cshtml");
        }
        var checkPasswordResult = await _userManager.CheckPasswordAsync(user, password);
        if (!checkPasswordResult)
        {
            return View("Views/Account/Login.cshtml");
        }

        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, _authService.CreateUserPrincipal(user));
        return RedirectToAction("Index", "Home");
    }

    [HttpPost("external-login")]
    public IActionResult ExternalLogin(string provider, string returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new {returnUrl});
        var properties = _authService.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [HttpGet("external-login-callback")]
    public async Task<IActionResult> ExternalLoginCallback()
    {
        //var info = await _authService.GetExternalLoginInfoAsync();
        var auth = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        if (!auth.Succeeded)
        {
            return RedirectToAction("Login");
        }
        var items = auth?.Properties?.Items;
        var provider = items[AuthService.LoginProviderKey];
        var providerKey = auth.Principal.FindFirstValue("sub");
        var user = await _userManager.FindByLoginAsync(provider, providerKey);
        if (user == null)
        {
            var email = auth.Principal.FindFirstValue(ClaimTypes.Email);
            user = new () {
                Email = email,
                EmailConfirmed = true,
                UserName = email
            };
            var creatResult = await _userManager.CreateAsync(user);
            var addLoginResult = await _userManager.AddLoginAsync(user, new (provider, providerKey, provider));
        }
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, _authService.CreateUserPrincipal(user));
        return RedirectToAction("Index", "Home");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        return SignOut(new AuthenticationProperties {
            RedirectUri = "/"
        }, IdentityConstants.ApplicationScheme);
    }
}
