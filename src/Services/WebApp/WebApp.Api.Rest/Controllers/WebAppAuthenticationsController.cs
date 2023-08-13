using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Rest.Models.WebApps.Authentications;

namespace MycroCloud.WebApp.Api.Rest.Controllers;

[Authorize]
[Route("webapp/{WebAppName}/authentications")]
public class WebApplicationAuthenticationsController: BaseApiController
{
    [HttpGet("settings")]
    public async Task<IActionResult> Settings(int WebApplicationId)
    {
        return Ok();
    }

    [HttpPost("settings")]
    public async Task<IActionResult> Settings(int WebApplicationId, AuthenticationSettingsModel model)
    {
        return Ok();
    }

    [HttpGet("schemes")]
    public async Task<IActionResult> Schemes(int WebApplicationId)
    {
        return Ok();
    }

    [HttpGet("schemes/jwtbearer/new")]
    public async Task<IActionResult> NewJwtBearerScheme(int WebApplicationId)
    {
        return Ok();
    }

    [HttpPost("schemes/jwtbearer/new")]
    public async Task<IActionResult> NewJwtBearerScheme(int WebApplicationId, string WebApplicationName, JwtBearerSchemeSaveModel model)
    {
        return Ok();
    }

    [HttpGet("schemes/jwtbearer/edit/{SchemeId:int}")]
    public async Task<IActionResult> EditJwtBearerScheme(int WebApplicationId, int SchemeId)
    {
        return Ok();
    }

    [HttpPost("schemes/jwtbearer/edit/{SchemeId:int}")]
    public async Task<IActionResult> EditJwtBearerScheme(string WebApplicationName, int SchemeId, JwtBearerSchemeSaveModel model)
    {
        return Ok();
    }

    [HttpGet("schemes/apikey/new")]
    public async Task<IActionResult> NewApiKeyScheme(int WebApplicationId)
    {
        return Ok();
    }

    [HttpGet("schemes/apikey/edit/{SchemeId:int}")]
    public async Task<IActionResult> EditApiKeyScheme(int WebApplicationId, int SchemeId)
    {
        return Ok();
    }
}
