using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockServer.Core.Enums;
using MockServer.Web.Filters;
using MockServer.Web.Models.ProjectSettings.Auth;
using MockServer.Web.Services.Interfaces;
using RouteName = MockServer.Web.Common.Constants.RouteName;
namespace MockServer.Web.Controllers;

[Authorize]
[Route("projects/{ProjectName}/settings")]
[GetAuthUserProjectId(RouteName.ProjectName, RouteName.ProjectId)]
public class ProjectSettingsController : BaseController
{
    private readonly IProjectWebService _projectService;
    private readonly IProjectSettingsWebService _settingsService;
    public ProjectSettingsController(IProjectWebService projectService,
        IProjectSettingsWebService settingsService)
    {
        _projectService = projectService;
        _settingsService = settingsService;
    }

    [HttpGet]
    [HttpGet("general")]
    public async Task<IActionResult> Index(int ProjectId)
    {
        var model = await _settingsService.GetIndexModel(ProjectId);
        return View("Views/ProjectSettings/Index.cshtml", model);
    }

    [HttpGet("auth")]
    public async Task<IActionResult> Auth(int ProjectId)
    {
        var model = await _settingsService.GetAuthIndexModel(ProjectId);
        return View("Views/ProjectSettings/Auth/Index.cshtml", model);
    }
    [HttpPost("auth")]
    public async Task<IActionResult> SaveAuth(string ProjectName, int ProjectId, AuthIndexModel model)
    {
        await _settingsService.SaveAuthIndexModel(ProjectId, model);
        return RedirectToAction(nameof(Auth), new { name = ProjectName });
    }

    [HttpGet("auth/jwtbearer/{id:int?}")]
    public async Task<IActionResult> ViewJwtBearer(string name, int? id)
    {
        var model = await _settingsService.GetJwtBearerAuthModel(name, id ?? 0);
        return View("Views/ProjectSettings/Auth/JWT/Create.cshtml", model);
    }

    [HttpPost("auth/jwtbearer/{id:int?}")]
    public async Task<IActionResult> CreateJwtBearer(string name, int? id, JwtBearerAuthModel model)
    {
        if (id is not int)
        {
            await _settingsService.CreateJwtBearerAuthentication(ProjectId, model);
        }
        else
        {
            Request.RouteValues.Remove("id");
            await _settingsService.EditJwtBearerAuthentication(id.Value, model);
        }
        return RedirectToAction(nameof(Auth), Request.RouteValues);
    }

    [HttpGet("auth/jwtbearer/{id:int}/generate-token")]
    public async Task<IActionResult> GenerateJwtBearerToken(string name, int id)
    {
        var model = new JwtBearerTokenGenerateModel();
        return View("Views/ProjectSettings/Auth/JWT/GenerateToken.cshtml", model);
    }

    [HttpPost("auth/jwtbearer/{id:int}/generate-token")]
    public async Task<IActionResult> GenerateJwtBearerToken(string name, int id, JwtBearerTokenGenerateModel model)
    {
        var token = await _settingsService.GenerateJwtBearerToken(name, id, model);
        return View("Views/ProjectSettings/Auth/JWT/GenerateToken.cshtml", token);
    }

    [HttpGet("auth/apikey/create")]
    public async Task<IActionResult> CreateApiKey(string name)
    {
        var model = await _settingsService.GetApiKeyAuthModel(name, 0);
        return View("Views/ProjectSettings/Auth/ApiKey/Create.cshtml", model);
    }

    [HttpPost("auth/apikey/create")]
    public async Task<IActionResult> CreateApiKey(string name, ApiKeyAuthModel model)
    {
        await _settingsService.CreateApiKeyAuthentication(name, model);
        return RedirectToAction(nameof(Auth), Request.RouteValues);
    }

    [HttpPost("auth/apikey/{id:int}/generate-key")]
    public async Task<IActionResult> GenerateApiKey(string name, int id)
    {
        string key = await _projectService.GenerateKey(ProjectId);
        return RedirectToAction(nameof(Auth), Request.RouteValues);
    }

    [HttpGet("auth/apikey/{id:int}")]
    public async Task<IActionResult> ViewApiKey(string name, int id)
    {
        var model = await _settingsService.GetApiKeyAuthModel(name, id);
        return View("Views/ProjectSettings/Auth/ApiKey/Create.cshtml", model);
    }

    [HttpPost("rename")]
    public async Task<IActionResult> Rename(int ProjectId, string newName)
    {
        await _projectService.Rename(ProjectId, newName);
        return RedirectToAction(nameof(Index), new { name = newName });
    }

    [HttpPost("set-accessibility")]
    public async Task<IActionResult> SetAccessibility(string name, ProjectAccessibility accessibility)
    {
        await _projectService.SetAccessibility(name, accessibility);
        return RedirectToAction(nameof(View), Request.RouteValues);
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete(string name)
    {
        await _projectService.Delete(name);
        return RedirectToAction(nameof(Index));
    }
}