using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockServer.Core.Enums;
using MockServer.Core.Models.Authorization;
using MockServer.WebMVC.Attributes;
using MockServer.WebMVC.Extentions;
using MockServer.WebMVC.Services.Interfaces;

namespace MockServer.WebMVC.Controllers;

[Authorize]
[Route("projects/{name}/settings")]
public class ProjectSettingsController : BaseController
{
    private readonly IProjectService _projectService;

    public ProjectSettingsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpPost("rename")]
    public async Task<IActionResult> Rename(string name, string newName)
    {
        await _projectService.Rename(name, newName);
        return !Request.IsAjaxRequest() ? RedirectToAction(nameof(View), new { name = newName }) :
        Ok();
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

    [AjaxOnly]
    [HttpPost("auth/jwt/create")]
    public async Task<IActionResult> CreateJwtHandler(string name, JwtHandlerConfiguration configuration)
    {
        await _projectService.CreateJwtHandler(name, configuration);
        return Ok();
    }
}