using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockServer.Web.Filters;
using MockServer.Web.Models.ProjectAuthentication;
using MockServer.Web.Models.ProjectAuthentication.JwtBearer;
using MockServer.Web.Services.Interfaces;
using static MockServer.Web.Common.Constants;

namespace MockServer.Web.Controllers;

[Authorize]
[Route("projects/{ProjectName}/authentication")]
[GetAuthUserProjectId(RouteName.ProjectName, RouteName.ProjectId)]
public class ProjectAuthenticationController: BaseController
{
    private readonly IProjectAuthenticationWebService _service;

    public ProjectAuthenticationController(IProjectAuthenticationWebService projectAuthenticationWebService)
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
    public async Task<IActionResult> Configure(int ProjectId, IndexViewModel model)
    {
        await _service.SaveConfigurations(ProjectId, model);
        return RedirectToAction(nameof(Index), Request.RouteValues);
    }

    [HttpGet("jwtbearer/{SchemeId:int?}")]
    public async Task<IActionResult> JwtBearer(int ProjectId, int? SchemeId)
    {
        var model = await _service.GetJwtBearerScheme(ProjectId, SchemeId ?? 0);
        return View("Views/ProjectSettings/Auth/JWT/Create.cshtml", model);
    }

    [HttpPost("jwtbearer/{SchemeId:int?}")]
    public async Task<IActionResult> JwtBearer(int ProjectId, int? SchemeId, JwtBearerSchemeViewModel model)
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
        var model = await _service.GetJwtBearerScheme(ProjectId, SchemeId ?? 0);
        return View("Views/ProjectSettings/Auth/JWT/Create.cshtml", model);
    }
}
