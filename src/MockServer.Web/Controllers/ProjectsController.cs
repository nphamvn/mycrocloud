using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockServer.Web.Filters;
using MockServer.Web.Models.Projects;
using MockServer.Web.Services.Interfaces;
using RouteName = MockServer.Web.Common.Constants.RouteName;
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

    [HttpGet("{ProjectName}")]
    [GetAuthUserProjectId(RouteName.ProjectName, RouteName.ProjectId)]
    public async Task<IActionResult> View(int ProjectId)
    {
        var vm = await _projectService.GetProjectViewViewModel(ProjectId);
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
}