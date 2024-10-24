using Microsoft.AspNetCore.Mvc;
using WebApp.RestApi.Filters;
using WebApp.RestApi.Services;

namespace WebApp.RestApi.Controllers;

[Route("[controller]")]
public class WebhooksController(RabbitMqService rabbitMqService) : BaseController
{
    [HttpPost("github")]
    [ServiceFilter(typeof(GitHubWebhookValidationFilter))]
    public async Task<IActionResult> ReceiveGitHubEvent()
    {
        var message = "GitHub event received.";
        rabbitMqService.PublishMessage(message);
        return Ok();
    }
}