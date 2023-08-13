using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MycroCloud.WebMvc.Areas.Services.Models.WebApps;
using MycroCloud.WebMvc.Areas.Services.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MycroCloud.WebMvc.Areas.Services.Controllers;

[Authorize]
[Route("webapp/{WebAppName}/authentications")]
public class WebAppAuthenticationController
    (IWebAppAuthenticationService webAppAuthenticationService) : BaseServiceController
{
    public const string Name = "WebAppAuthentication";
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        ViewData["WebAppName"] = context.ActionArguments["WebAppName"];
    }

    [HttpGet("Configurations")]
    public async Task<IActionResult> Configurations(int WebApplicationId)
    {
        var vm = await webAppAuthenticationService.GetConfigurationsViewModel(WebApplicationId);
        return View("/Areas/Services/Views/WebApp/Authentication/Configuration.cshtml", vm);
    }

    [HttpPost("Configurations")]
    public async Task<IActionResult> Configurations(int WebApplicationId, AuthenticationConfigurationViewModel viewModel)
    {
        await webAppAuthenticationService.SaveSettings(WebApplicationId, viewModel);
        return RedirectToAction(nameof(Configurations), Request.RouteValues);
    }

    [HttpGet("Schemes")]
    public async Task<IActionResult> SchemeList(int WebAppId)
    {
        var vm = await webAppAuthenticationService.GetSchemeListViewModel(WebAppId);
        return View("/Areas/Services/Views/WebApp/Authentication/SchemeList.cshtml", vm);
    }

    [HttpGet("Schemes/JwtBearer/Create")]
    public async Task<IActionResult> CreateJwtBearerScheme(int WebApplicationId)
    {
        var model = await webAppAuthenticationService.GetCreateJwtBearerSchemeModel(WebApplicationId);
        return View("/Areas/Services/Views/WebApp/Authentication/SaveJwtBearerScheme.cshtml", model);
    }

    [HttpPost("Schemes/JwtBearer/Create")]
    public async Task<IActionResult> CreateJwtBearerScheme(int WebApplicationId, string WebAppName, JwtBearerAuthenticationSchemeSaveViewModel model)
    {
        await webAppAuthenticationService.AddJwtBearerScheme(WebApplicationId, model);
        return RedirectToAction(nameof(SchemeList), new { WebAppName });
    }

    [HttpGet("Schemes/JwtBearer/{SchemeId:int}/Edit")]
    public async Task<IActionResult> EditJwtBearerScheme(int WebApplicationId, int SchemeId)
    {
        var model = await webAppAuthenticationService.GetEditJwtBearerSchemeModel(WebApplicationId, SchemeId);
        return View("/Areas/Services/Views/WebApp/Authentication/SaveJwtBearerScheme.cshtml", model);
    }

    [HttpGet("Schemes/ApiKey/Create")]
    public async Task<IActionResult> CreateApiKeyScheme(int WebApplicationId)
    {
        var model = await webAppAuthenticationService.GetCreateJwtBearerSchemeModel(WebApplicationId);
        return View("/Areas/Services/Views/WebApp/Authentication/SaveJwtBearerScheme.cshtml", model);
    }

    [HttpPost("Schemes/ApiKey/Create")]
    public async Task<IActionResult> CreateApiKeyScheme(int WebApplicationId, string WebAppName, JwtBearerAuthenticationSchemeSaveViewModel model)
    {
        await webAppAuthenticationService.AddJwtBearerScheme(WebApplicationId, model);
        return RedirectToAction(nameof(SchemeList), new { WebAppName });
    }

    [HttpGet("Schemes/ApiKey/{SchemeId:int}/Edit")]
    public async Task<IActionResult> EditApiKeyScheme(int WebApplicationId, int SchemeId)
    {
        var model = await webAppAuthenticationService.GetEditJwtBearerSchemeModel(WebApplicationId, SchemeId);
        return View("/Areas/Services/Views/WebApp/Authentication/SaveJwtBearerScheme.cshtml", model);
    }

    [HttpPost("Schemes/ApiKey/{SchemeId:int}/Edit")]
    public async Task<IActionResult> EditApiKeyScheme(string WebAppName, int SchemeId, JwtBearerAuthenticationSchemeSaveViewModel model)
    {
        await webAppAuthenticationService.EditJwtBearerScheme(SchemeId, model);
        return RedirectToAction(nameof(SchemeList), new { WebAppName });
    }
}
