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

    [HttpPost("rename")]
    public async Task<IActionResult> Rename(int ProjectId, string newName)
    {
        await _projectService.Rename(ProjectId, newName);
        return RedirectToAction(nameof(Index), new { name = newName });
    }

    [HttpPost("set-accessibility")]
    public async Task<IActionResult> SetAccessibility(int ProjectId, ProjectAccessibility accessibility)
    {
        await _projectService.SetAccessibility(ProjectId, accessibility);
        return RedirectToAction(nameof(View), Request.RouteValues);
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete(int ProjectId)
    {
        await _projectService.Delete(ProjectId);
        return RedirectToAction(nameof(Index));
    }
}