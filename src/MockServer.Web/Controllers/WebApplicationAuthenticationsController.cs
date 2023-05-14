using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockServer.Core.WebApplications.Security;
using MockServer.Web.Filters;
using MockServer.Web.Models.WebApplications.Authentications;
using MockServer.Web.Models.WebApplications.Authentications.JwtBearer;
using MockServer.Web.Services;
using static MockServer.Web.Common.Constants;
namespace MockServer.Web.Controllers;

[Authorize]
[Route("webapps/{WebApplicationName}/authentications")]
[GetAuthUserWebApplicationId(RouteName.WebApplicationName, RouteName.WebApplicationId)]
public class WebApplicationAuthenticationsController: BaseController
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
    public async Task<IActionResult> NewJwtBearerScheme(int WebApplicationId, JwtBearerSchemeSaveModel model)
    {
        await _webApplicationAuthenticationWebService.AddJwtBearerScheme(WebApplicationId, model);
        return RedirectToAction(nameof(Schemes), Request.RouteValues);
    }

    [HttpGet("schemes/jwtbearer/edit/{SchemeId:int}")]
    public async Task<IActionResult> EditJwtBearerScheme(int WebApplicationId, int SchemeId)
    {
        var model = await _webApplicationAuthenticationWebService.GetEditJwtBearerSchemeModel(WebApplicationId, SchemeId);
        return View("/Views/WebApplications/Authentications/SaveJwtBearerScheme.cshtml", model);
    }

    [HttpPost("schemes/jwtbearer/edit/{SchemeId:int}")]
    public async Task<IActionResult> EditJwtBearerScheme(int WebApplicationId, JwtBearerSchemeSaveModel model)
    {
        await _webApplicationAuthenticationWebService.AddJwtBearerScheme(WebApplicationId, model);
        return RedirectToAction(nameof(Schemes), Request.RouteValues);
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
