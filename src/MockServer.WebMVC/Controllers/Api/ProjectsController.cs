using Microsoft.AspNetCore.Mvc;
using MockServer.WebMVC.Services.Interfaces;

namespace MockServer.WebMVC.Controllers.Api;

public class ProjectsController : ApiController
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    // [HttpPost("rename")]
    // public async Task<IActionResult> Rename(string name, string newName)
    // {
    //     await _projectService.Rename(name, newName);
    //     return Ok();
    // }
}