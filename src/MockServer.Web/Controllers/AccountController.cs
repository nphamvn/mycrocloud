using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MockServer.Web.Services;

namespace MockServer.Web.Controllers;

[Authorize]
[Route("[controller]")]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public AccountController(ILogger<AccountController> logger
        ,UserManager<IdentityUser> userManager
        , IAuthService authService
        , IConfiguration configuration
        )
    {
        _logger = logger;
        _userManager = userManager;
        _authService = authService;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpGet("login")]
    public async Task<IActionResult> Login(string returnUrl)
    {
        return View("/Views/Account/Login.cshtml");
    }

    [AllowAnonymous]
    [HttpPost("local-login")]
    public async Task<IActionResult> LocalLogin(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return View("/Views/Account/Login.cshtml");
        }
        var checkPasswordResult = await _userManager.CheckPasswordAsync(user, password);
        if (!checkPasswordResult)
        {
            return View("/Views/Account/Login.cshtml");
        }
        var principal = _authService.CreateUserPrincipal(user);
        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    [HttpPost("external-login")]
    public IActionResult ExternalLogin(string provider, string returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new {returnUrl});
        var properties = _authService.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return Challenge(properties, provider);
    }

    [AllowAnonymous]
    [HttpGet("external-login-callback")]
    public async Task<IActionResult> ExternalLoginCallback()
    {
        var auth = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
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
            var email = auth.Principal.FindFirstValue("email");
            user = new () {
                Email = email,
                EmailConfirmed = true,
                UserName = email
            };
            var creatResult = await _userManager.CreateAsync(user);
            var addLoginResult = await _userManager.AddLoginAsync(user, new (provider, providerKey, provider));
        }
        var principal = _authService.CreateUserPrincipal(user);
        await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        return SignOut(new AuthenticationProperties {
            RedirectUri = "/"
        }, IdentityConstants.ApplicationScheme);
    }
    
    [HttpGet("settings")]
    public async Task<IActionResult> Settings()
    {
        return View("/Views/Account/Settings.cshtml");
    }

    [HttpPost("connect/github")]
    public async Task<IActionResult> ConnectGitHub(){
        var redirect_uri = Url.ActionLink(nameof(ConnectGitHubCallback));
        var scope = string.Join(' ', new List<string> {
            "user",
            "repo",
        });
        var redirectUrl = $"https://github.com/login/oauth/authorize?client_id={_configuration["GitHub:ClientId"]}&redirect_uri={redirect_uri}&scope={scope}";
        return Redirect(redirectUrl);
    }

    [Route("connect/github-callback")]
    public async Task<IActionResult> ConnectGitHubCallback(string code, string state){
        //exchange code for access token
        using (var client = new HttpClient())
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", _configuration["GitHub:ClientId"] },
                { "client_secret", _configuration["GitHub:ClientSecret"] },
                { "code", code },
                { "state", state }
            });

            var response = await client.PostAsync("https://github.com/login/oauth/access_token", content);
            var responseString = await response.Content.ReadAsStringAsync();

            //var gitHubResponse = await response.Content.ReadFromJsonAsync<GitHubResponse>();
            // Save the access token in database for further use using Microsoft.AspNetCore.Identity
            System.IO.File.WriteAllText(nameof(responseString), responseString);
            return RedirectToAction(nameof(Settings));
        }
    }
}
