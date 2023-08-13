using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MycroCloud.WebApp.Api.Rest.Controllers;

[Authorize]
[Route("webapps/{WebApplicationName}/authentications")]
public class WebApplicationAuthenticationsController: BaseApiController
{
    private readonly IWebApplicationAuthenticationWebService _webApplicationAuthenticationWebService;

    public WebApplicationAuthenticationsController(IWebApplicationAuthenticationWebService webApplicationAuthenticationWebService)
    {
        _webApplicationAuthenticationWebService = webApplicationAuthenticationWebService;
    }

    [HttpGet("settings")]
    public async Task<IActionResult> Settings(int WebApplicationId)
    {
        var model = await _webApplicationAuthenticationWebService.GetAuthenticationSettingsModel(WebApplicationId);
        return View("/Views/WebApplications/Authentications/Settings.cshtml", model);
    }

    [HttpPost("settings")]
    public async Task<IActionResult> Settings(int WebApplicationId, AuthenticationSettingsModel model)
    {
        await _webApplicationAuthenticationWebService.SaveSettings(WebApplicationId, model);
        return RedirectToAction(nameof(Settings), Request.RouteValues);
    }

    [HttpGet("schemes")]
    public async Task<IActionResult> Schemes(int WebApplicationId)
    {
        var model = await _webApplicationAuthenticationWebService.GetIndexViewModel(WebApplicationId);
        return View("/Views/WebApplications/Authentications/SchemeList.cshtml", model);
    }

    [HttpGet("schemes/jwtbearer/new")]
    public async Task<IActionResult> NewJwtBearerScheme(int WebApplicationId)
    {
        var model = await _webApplicationAuthenticationWebService.GetCreateJwtBearerSchemeModel(WebApplicationId);
        return View("/Views/WebApplications/Authentications/SaveJwtBearerScheme.cshtml", model);
    }

    [HttpPost("schemes/jwtbearer/new")]
    public async Task<IActionResult> NewJwtBearerScheme(int WebApplicationId, string WebApplicationName, JwtBearerSchemeSaveModel model)
    {
        await _webApplicationAuthenticationWebService.AddJwtBearerScheme(WebApplicationId, model);
        return RedirectToAction(nameof(Schemes), new { WebApplicationName });
    }

    [HttpGet("schemes/jwtbearer/edit/{SchemeId:int}")]
    public async Task<IActionResult> EditJwtBearerScheme(int WebApplicationId, int SchemeId)
    {
        var model = await _webApplicationAuthenticationWebService.GetEditJwtBearerSchemeModel(WebApplicationId, SchemeId);
        return View("/Views/WebApplications/Authentications/SaveJwtBearerScheme.cshtml", model);
    }

    [HttpPost("schemes/jwtbearer/edit/{SchemeId:int}")]
    public async Task<IActionResult> EditJwtBearerScheme(string WebApplicationName, int SchemeId, JwtBearerSchemeSaveModel model)
    {
        await _webApplicationAuthenticationWebService.EditJwtBearerScheme(SchemeId, model);
        return RedirectToAction(nameof(Schemes), new { WebApplicationName });
    }

    [HttpGet("schemes/apikey/new")]
    public async Task<IActionResult> NewApiKeyScheme(int WebApplicationId)
    {
        var model = await _webApplicationAuthenticationWebService.GetCreateJwtBearerSchemeModel(WebApplicationId);
        return View("/Views/WebApplications/Authentications/SaveJwtBearerScheme.cshtml", model);
    }

    [HttpGet("schemes/apikey/edit/{SchemeId:int}")]
    public async Task<IActionResult> EditApiKeyScheme(int WebApplicationId, int SchemeId)
    {
        var model = await _webApplicationAuthenticationWebService.GetEditJwtBearerSchemeModel(WebApplicationId, SchemeId);
        return View("/Views/WebApplications/Authentications/SaveJwtBearerScheme.cshtml", model);
    }
}
