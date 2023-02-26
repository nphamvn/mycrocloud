using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockServer.Web.Filters;
using MockServer.Web.Models.WebApplications.Authentications;
using MockServer.Web.Models.WebApplications.Authentications.JwtBearer;
using MockServer.Web.Services;
using static MockServer.Web.Common.Constants;
namespace MockServer.Web.Controllers;

[Authorize]
[Route("webapps/{ProjectName}/authentications")]
[GetAuthUserProjectId(RouteName.ProjectName, RouteName.ProjectId)]
public class WebApplicationAuthenticationsController: BaseController
{
    private readonly IWebApplicationAuthenticationWebService _service;

    public WebApplicationAuthenticationsController(IWebApplicationAuthenticationWebService projectAuthenticationWebService)
    {
        _service = projectAuthenticationWebService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int ProjectId)
    {
        var model = await _service.GetIndexViewModel(ProjectId);
        return View("Views/ProjectSettings/Auth/Index.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(int ProjectId, AuthenticationIndexModel model)
    {
        await _service.SaveConfigurations(ProjectId, model);
        return RedirectToAction(nameof(Index), Request.RouteValues);
    }

    [HttpGet("jwtbearer/{SchemeId:int?}")]
    public async Task<IActionResult> CreateJwtBearer(int ProjectId, int? SchemeId)
    {
        var model = await _service.GetJwtBearerScheme(SchemeId ?? 0);
        return View("Views/ProjectSettings/Auth/JWT/Create.cshtml", model);
    }

    [HttpPost("jwtbearer/{SchemeId:int?}")]
    public async Task<IActionResult> JwtBearer(int ProjectId, int? SchemeId, JwtBearerSchemeSaveModel model)
    {
        if (SchemeId is not int)
        {
            await _service.AddJwtBearerScheme(ProjectId, model);
        }
        else
        {
            await _service.EditJwtBearerScheme(SchemeId.Value, model);
        }
        return RedirectToAction(nameof(Index), Request.RouteValues);
    }

    [HttpGet("apikey/{SchemeId:int?}")]
    public async Task<IActionResult> ApiKey(int ProjectId, int? SchemeId)
    {
        var model = await _service.GetJwtBearerScheme(SchemeId ?? 0);
        return View("Views/ProjectSettings/Auth/JWT/Create.cshtml", model);
    }
}
