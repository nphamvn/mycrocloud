using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Infrastructure;
using WebApp.RestApi.Extensions;

namespace WebApp.RestApi.Controllers;

[Route("[controller]")]
public class IntegrationsController(AppDbContext appDbContext, IConfiguration configuration, ILogger<IntegrationsController> logger) : BaseController
{
    [HttpPost("github/callback")]
    public async Task<IActionResult> GitHubCallback(GitHubAuthRequest request)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        var requestData = new Dictionary<string, string>
        {
            { "client_id", configuration["OAuthApps:GitHub:ClientId"]! },
            { "client_secret", configuration["OAuthApps:GitHub:ClientSecret"]! },
            { "code", request.Code }
        };
        var response = await client.PostAsync("https://github.com/login/oauth/access_token", new FormUrlEncodedContent(requestData));
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<GitHubAuthResponse>(responseBody)!;
        
        var existingToken = await appDbContext.UserTokens
            .Where(t => t.UserId == User.GetUserId() && t.Provider == "GitHub" &&
                        t.Purpose == UserTokenPurpose.AppIntegration)
            .SingleOrDefaultAsync();
        
        if (existingToken is not null)
        {
            existingToken.Token = authResponse.AccessToken;
            existingToken.UpdatedAt = DateTime.UtcNow;
            appDbContext.UserTokens.Update(existingToken);
        }
        else
        {
            await appDbContext.UserTokens.AddAsync(new UserToken()
            {
                UserId = User.GetUserId(),
                Provider = "GitHub",
                Purpose = UserTokenPurpose.AppIntegration,
                CreatedAt = DateTime.UtcNow,
                Token = authResponse.AccessToken
            });
        }
        
        await appDbContext.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpGet("github/repos")]
    public async Task<IActionResult> GetGitHubRepos()
    {
        var userToken = await appDbContext.UserTokens
            .Where(t => t.UserId == User.GetUserId() && t.Provider == "GitHub" &&
                        t.Purpose == UserTokenPurpose.AppIntegration)
            .SingleOrDefaultAsync();

        if (userToken is null)
        {
            return Unauthorized();
        }
        
        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/repos");
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("WebApp", "1.0"));
        request.Headers.Add("Accept", "application/vnd.github+json");
        request.Headers.Add("Authorization", "Bearer " + userToken.Token);
        request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var repos = JsonSerializer.Deserialize<List<GitHubRepo>>(responseBody)!;
        return Ok(repos.Select(repo => new
        {
            repo.Id,
            repo.Name,
            repo.FullName,
            repo.Description,
            repo.CreatedAt,
            repo.UpdatedAt
        }));
    }

    [HttpPost("app-github")]
    public async Task<IActionResult> ConnectAppGitHub(int appId, string repoFullName, bool connect)
    {
        var userToken = await appDbContext.UserTokens
            .Where(t => t.UserId == User.GetUserId() && t.Provider == "GitHub" &&
                        t.Purpose == UserTokenPurpose.AppIntegration)
            .SingleOrDefaultAsync();
        if (userToken is null)
        {
            return Unauthorized();
        }
        
        var app = await appDbContext.Apps.SingleAsync(a => a.Id == appId);
        
        if (connect)
        {
            // Fetch repo details
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/repositories/{repoFullName}");
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("WebApp", "1.0"));
            request.Headers.Add("Accept", "application/vnd.github+json");
            request.Headers.Add("Authorization", "Bearer " + userToken.Token);
            request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var repo = JsonSerializer.Deserialize<GitHubRepo>(responseBody)!;

            // Add webhook
            var config = configuration.GetSection("AppIntegrations:GitHubWebhook");
            var webhookRequest = new
            {
                events = config.GetValue<string[]>("Events"),
                config = new
                {
                    url = config["Config:Url"]!.TrimEnd('/') + $"/{appId}",
                    content_type = "json"
                },
                secret = config["Config:Secret"]
            };
            var webhookResponse = await client.PostAsync($"https://api.github.com/repos/{repo.FullName}/hooks", new StringContent(JsonSerializer.Serialize(webhookRequest), Encoding.UTF8, "application/json"));
            webhookResponse.EnsureSuccessStatusCode();
        }
        else
        {
            // Remove webhook
            using var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.github.com/repositories/{app.GitHubRepoFullName}/hooks");
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("WebApp", "1.0"));
            request.Headers.Add("Accept", "application/vnd.github+json");
            request.Headers.Add("Authorization", "Bearer " + userToken.Token);
            request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
        
        app.GitHubRepoFullName = connect ? repoFullName : null;
        await appDbContext.SaveChangesAsync();
        return NoContent();
    }
}

public class GitHubRepo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("full_name")]
    public string FullName { get; set; }
    
    [JsonPropertyName("description")]
    public string Description { get; }
    
    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
}

public class GitHubAuthRequest
{
    public string Code { get; set; }
}

public class GitHubAuthResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
}