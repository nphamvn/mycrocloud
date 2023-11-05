using System.Text.Json;
using Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private const string client_id = "5ac289bd76d5bc7546f6";
    private const string client_secret = "3dab60864a9e4a414e26380d4c4209bf02ea9fc5";

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }
    [HttpGet("AuthFlowInfo")]
    public async Task<IActionResult> GetAuthFlowInfo(string provider)
    {
        if (provider == "GitHub")
        {
            return Ok(new
            {
                ClientId = client_id,
                AuthorizeUrl = "https://github.com/login/oauth/authorize"
            });
        }

        return BadRequest();
    }
    
    [HttpPost("login/github")]
    [AllowAnonymous]
    public async Task<IActionResult> GitHubLogin(string code)
    {
        var authResult = JsonSerializer.Deserialize<GitHubAuthResponse>(await GetGitHubAccessToken(code));
        var user = await GetGitHubUserInfo(authResult!.access_token);
        return Ok(user);
    }

    private static async Task<string> GetGitHubUserInfo(string accessToken)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage();
        request.RequestUri = new Uri("https://api.github.com/user");
        request.Method = HttpMethod.Get;
        
        request.Headers.Add("User-Agent", "MycroCloud Idp");
        request.Headers.Add("Authorization", $"Bearer {accessToken}");

        var response = await client.SendAsync(request);
        var result = await response.Content.ReadAsStringAsync();
        return result;
    }
    
    private static async Task<string> GetGitHubAccessToken(string code)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage();
        request.RequestUri =
            new Uri(
                $"https://github.com/login/oauth/access_token?client_id={client_id}&client_secret={client_secret}&code={code}");
        request.Method = HttpMethod.Post;
        request.Headers.Add("Accept", "application/json");
        var response = await client.SendAsync(request);
        var result = await response.Content.ReadAsStringAsync();
        return result;
    }
}