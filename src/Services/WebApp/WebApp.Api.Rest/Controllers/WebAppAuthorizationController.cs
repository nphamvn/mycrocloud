using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Rest.Models.WebApps.Authentications;

namespace MycroCloud.WebApp.Api.Rest.Controllers;

[Authorize]
[Route("webapp/{WebAppName}/authorization")]
public class WebApplicationAuthorizationsController : BaseApiController
{
    [HttpGet("policies")]
    public async Task<IActionResult> PolicyIndex(int WebApplicationId)
    {
        return Ok();
    }

    [HttpGet("policies/create")]
    public async Task<IActionResult> PolicyCreate(int WebApplicationId)
    {
        return Ok();
    }

    [HttpPost("policies/create")]
    public async Task<IActionResult> PolicyCreate(string WebApplicationName, int WebApplicationId, PolicySaveModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }
        return Ok();
    }

    [HttpGet("policies/{PolicyId:int}/edit")]
    public async Task<IActionResult> PolicyEdit(int PolicyId)
    {
        return Ok();
    }

    [HttpPost("policies/{PolicyId:int}/edit")]
    public async Task<IActionResult> PolicyEdit(string WebApplicationName, int PolicyId, PolicySaveModel model)
    {
        return Ok();
    }

    [HttpPost("policies/{PolicyId:int}/delete")]
    public async Task<IActionResult> PolictDelete(string WebApplicationName, int PolicyId)
    {
        return Ok();
    }
}
