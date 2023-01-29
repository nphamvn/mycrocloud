using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockServer.Core.Enums;
using MockServer.Web.Attributes;
using MockServer.Web.Models.ProjectSettings.Auth;
using MockServer.Web.Services.Interfaces;

namespace MockServer.Web.Controllers;

[Authorize]
[Route("projects/{name}/settings")]
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
    public async Task<IActionResult> Index(string name)
    {
        var model = await _settingsService.GetIndexModel(name);
        return View("Views/ProjectSettings/Index.cshtml", model);
    }

    [HttpGet("auth")]
    public async Task<IActionResult> Auth(string name)
    {
        var model = await _settingsService.GetAuthIndexModel(name);
        return View("Views/ProjectSettings/Auth/Index.cshtml", model);
    }
    [HttpPost("auth")]
    public async Task<IActionResult> SaveAuth(string name, AuthIndexModel model)
    {
        await _settingsService.SaveAuthIndexModel(name, model);
        return RedirectToAction(nameof(Auth), new { name = name });
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
            await _settingsService.CreateJwtBearerAuthentication(name, model);
        }
        else
        {
            Request.RouteValues.Remove("id");
            await _settingsService.EditJwtBearerAuthentication(id.Value, model);
        }
        return RedirectToAction(nameof(Auth), Request.RouteValues);
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

    [HttpGet("auth/apikey/{id:int}")]
    public async Task<IActionResult> ViewApiKey(string name, int id)
    {
        var model = await _settingsService.GetApiKeyAuthModel(name, id);
        return View("Views/ProjectSettings/Auth/ApiKey/Create.cshtml", model);
    }

    [HttpPost("rename")]
    public async Task<IActionResult> Rename(string name, string newName)
    {
        await _projectService.Rename(name, newName);
        return RedirectToAction(nameof(Index), new { name = newName });
    }

    [AjaxOnly]
    [HttpPost("generate-key")]
    public async Task<IActionResult> GenerateKey(string name)
    {
        string key = await _projectService.GenerateKey(name);
        return Ok(new { key = key });
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