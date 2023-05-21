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
    private readonly IConfiguration _configuration;

    public AccountController(ILogger<AccountController> logger
        , IConfiguration configuration
        )
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet("settings")]
    public async Task<IActionResult> Settings()
    {
        return View("/Views/Account/Settings.cshtml");
    }

    [HttpPost("connect/github")]
    public async Task<IActionResult> ConnectGitHub()
    {
        var redirect_uri = Url.ActionLink(nameof(ConnectGitHubCallback));
        var scope = string.Join(' ', new List<string> {
            "user",
            "repo",
        });
        var redirectUrl = $"https://github.com/login/oauth/authorize?client_id={_configuration["GitHub:ClientId"]}&redirect_uri={redirect_uri}&scope={scope}";
        return Redirect(redirectUrl);
    }

    [Route("connect/github-callback")]
    public async Task<IActionResult> ConnectGitHubCallback(string code, string state)
    {
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
