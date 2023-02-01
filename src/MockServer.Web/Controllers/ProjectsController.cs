using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockServer.Web.Attributes;
using MockServer.Web.Models.Common;
using MockServer.Web.Models.Projects;
using MockServer.Web.Services.Interfaces;

namespace MockServer.Web.Controllers;

[Authorize]
public class ProjectsController : BaseController
{
    private readonly IProjectWebService _projectService;

    public ProjectsController(IProjectWebService projectService)
    {
        _projectService = projectService;
    }
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index(ProjectIndexViewModel fm)
    {
        var vm = await _projectService.GetIndexViewModel(fm.Search);
        return View("Views/Projects/Index.cshtml", vm);
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