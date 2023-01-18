using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockServer.Core.Enums;
using MockServer.Core.Models.Authorization;
using MockServer.WebMVC.Attributes;
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

    [HttpGet]
    public async Task<IActionResult> Index(string name)
    {
        var vm = await _projectService.GetProjectViewViewModel(name);
        ViewData["ProjectName"] = name;
        return View("Views/ProjectSettings/Index.cshtml", vm.ProjectInfo);
    }

    [HttpGet("auth")]
    public async Task<IActionResult> Auth(string name)
    {
        ViewData["ProjectName"] = name;
        var vm = await _projectService.GetProjectViewViewModel(name);
        return View("Views/ProjectSettings/Auth/Index.cshtml", vm.ProjectInfo);
    }

    [HttpGet("auth/jwt/create")]
    public async Task<IActionResult> CreateJwt(string name)
    {
        ViewData["ProjectName"] = name;
        return View("Views/ProjectSettings/Auth/JWT/Create.cshtml");
    }

    [HttpPost("auth/jwt/create")]
    public async Task<IActionResult> CreateJwt(string name, JwtHandlerConfiguration configuration)
    {
        await _projectService.CreateJwtHandler(name, configuration);
        return RedirectToAction(nameof(Auth), Request.RouteValues);
    }

    [HttpGet("auth/jwt/{id:int}")]
    public async Task<IActionResult> ViewJwt(string name, int id)
    {
        ViewData["ProjectName"] = name;
        return View("Views/ProjectSettings/Auth/JWT/Create.cshtml");
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