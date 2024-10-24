using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Infrastructure;
using WebApp.RestApi.Filters;
using WebApp.RestApi.Services;

namespace WebApp.RestApi.Controllers;

public class WebhooksController(AppDbContext appDbContext, RabbitMqService rabbitMqService) : BaseController
{
    [HttpPost("github")]
    [AllowAnonymous]
    [TypeFilter<GitHubWebhookValidationFilter>]
    public async Task<IActionResult> ReceiveGitHubEvent()
    {
        var appId = 1; //todo: get appId from the request
        var repoId = 1; //todo: get repoId from the request
        var fullName = "";
        var app = await appDbContext.Apps.SingleOrDefaultAsync(a => a.Id == appId);
        if (app is null)
        {
            return BadRequest();
        }

        var userToken = await appDbContext.UserTokens.SingleOrDefaultAsync(t => t.UserId == app.UserId
            && t.Provider == "GitHub" &&
            t.Purpose == UserTokenPurpose.AppIntegration
        );

        if (userToken is null)
        {
            return BadRequest();
        }

        var message = new BuildMessage
        {
            Id = Guid.NewGuid().ToString(),
            RepoFullName = fullName,
            CloneUrl = $"https://{userToken.Token}@github.com/{fullName}.git"
        };

        rabbitMqService.PublishMessage(JsonSerializer.Serialize(message));
        return Ok();
    }

    private class BuildMessage
    {
        public string Id { get; set; }
        public string RepoFullName { get; set; }
        public string CloneUrl { get; set; }
    }
}