using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockServer.WebMVC.Attributes;
using MockServer.WebMVC.Models.Common;
using MockServer.WebMVC.Models.Project;
using MockServer.WebMVC.Services.Interfaces;

namespace MockServer.WebMVC.Controllers;

[Authorize]
public class ProjectsController : BaseController
{
    private readonly IProjectWebService _projectService;

    public ProjectsController(IProjectWebService projectService)
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
}