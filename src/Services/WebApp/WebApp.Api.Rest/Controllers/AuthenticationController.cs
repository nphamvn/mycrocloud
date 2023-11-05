using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Models;

namespace WebApp.Api.Controllers;

[Route("apps/{appId:int}/authentication")]
public class AuthenticationsController: BaseController
{
    [HttpGet("settings")]
    public async Task<IActionResult> Settings(int appId)
    {
        return Ok();
    }

    [HttpPost("settings")]
    public async Task<IActionResult> Settings(int appId, AuthenticationSettingsModel model)
    {
        return Ok();
    }

    [HttpGet("schemes")]
    public async Task<IActionResult> Schemes(int appId)
    {
        return Ok();
    }

    [HttpGet("schemes/jwtbearer/new")]
    public async Task<IActionResult> NewJwtBearerScheme(int appId)
    {
        return Ok();
    }

    [HttpPost("schemes/jwtbearer/new")]
    public async Task<IActionResult> NewJwtBearerScheme(int appId, JwtBearerSchemeSaveModel model)
    {
        return Ok();
    }

    [HttpGet("schemes/jwtbearer/edit/{schemeId:int}")]
    public async Task<IActionResult> EditJwtBearerScheme(int appId, int schemeId)
    {
        return Ok();
    }

    [HttpPost("schemes/jwtbearer/edit/{schemeId:int}")]
    public async Task<IActionResult> EditJwtBearerScheme(int appId, int schemeId, JwtBearerSchemeSaveModel model)
    {
        return Ok();
    }

    [HttpGet("schemes/apikey/new")]
    public async Task<IActionResult> NewApiKeyScheme(int appId)
    {
        return Ok();
    }

    [HttpGet("schemes/apikey/edit/{schemeId:int}")]
    public async Task<IActionResult> EditApiKeyScheme(int appId, int schemeId)
    {
        return Ok();
    }
}
