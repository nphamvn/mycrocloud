using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockServer.Core.Entities.Requests;
using MockServer.Core.Enums;
using MockServer.WebMVC.Attributes;
using MockServer.WebMVC.Extentions;
using MockServer.WebMVC.Models.Common;
using MockServer.WebMVC.Models.Project;
using MockServer.WebMVC.Services.Interfaces;

namespace MockServer.WebMVC.Controllers;

[Authorize]
public class ProjectsController : BaseController
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }
    [HttpGet]
    public async Task<IActionResult> Index(ProjectIndexViewModel fm)
    {
        var vm = await _projectService.GetIndexViewModel(fm.Search);
        return View("Views/Projects/Index.cshtml", vm);
    }

    [AjaxOnly]
    [HttpGet]
    public async Task<IActionResult> AjaxIndex(ProjectIndexViewModel fm)
    {
        var vm = await _projectService.GetIndexViewModel(fm.Search);
        return Ok(new AjaxResult<ProjectIndexViewModel>
        {
            Data = vm
        });
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> View(string name)
    {
        var vm = await _projectService.GetProjectViewViewModel(name);
        return View("Views/Projects/View.cshtml", vm);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        var vm = new CreateProjectViewModel();
        return View("Views/Projects/Create.cshtml", vm);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateProjectViewModel project)
    {
        if (!ModelState.IsValid)
        {
            return View("Views/Projects/Create.cshtml", project);
        }
        if (!await _projectService.Create(project))
        {
            return View("Views/Projects/Create.cshtml", project);
        }

        return RedirectToAction(nameof(View), new { name = project.Name });
    }

    [AjaxOnly]
    [HttpPost("create")]
    public async Task<IActionResult> CreateAjax(CreateProjectViewModel project)
    {
        if (!ModelState.IsValid)
        {
            var result = new AjaxResult<CreateProjectViewModel>
            {
                Data = project
            };
            var errors = ModelState.Select(x => x.Value.Errors).ToList();
            return BadRequest(result);
        }
        if (!await _projectService.Create(project))
        {
            return Ok();
        }

        return RedirectToAction(nameof(View), new { name = project.Name });
    }

    [HttpPost("{name}/settings/rename")]
    public async Task<IActionResult> Rename(string name, string newName)
    {
        await _projectService.Rename(name, newName);
        return !Request.IsAjaxRequest() ? RedirectToAction(nameof(View), new { name = newName }) :
        Ok();
    }

    [AjaxOnly]
    [HttpPost("{name}/settings/generate-key")]
    public async Task<IActionResult> GenerateKey(string name)
    {
        string key = await _projectService.GenerateKey(name);
        return Ok(new { key = key });
    }

    [HttpPost("{name}/settings/set-accessibility")]
    public async Task<IActionResult> SetAccessibility(string name, ProjectAccessibility accessibility)
    {
        await _projectService.SetAccessibility(name, accessibility);
        return RedirectToAction(nameof(View), Request.RouteValues);
    }

    [HttpPost("{name}/settings/delete")]
    public async Task<IActionResult> Delete(string name)
    {
        await _projectService.Delete(name);
        return RedirectToAction(nameof(Index));
    }

    [AjaxOnly]
    [HttpGet("{name}/requests/{id:int}")]
    public async Task<IActionResult> GetRequestEditorParital(string name, int id)
    {
        var vm = await _projectService.GetRequestOpenViewModel(name, id);
        vm.ProjectName = name;
        vm.RequestParams.Add(new RequestParam());
        vm.RequestHeaders.Add(new RequestHeader());
        vm.ResponseHeaders.Add(new ResponseHeader());
        return PartialView("Views/Projects/_RequestOpen.cshtml", vm);
    }
}