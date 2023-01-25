using Microsoft.AspNetCore.Mvc;
using MockServer.WebMVC.Models.Common;
using MockServer.WebMVC.Models.Project;
using MockServer.WebMVC.Services.Interfaces;

namespace MockServer.WebMVC.Controllers.Api;

public class ProjectsController : ApiController
{
    private readonly IProjectWebService _projectService;

    public ProjectsController(IProjectWebService projectService)
    {
        _projectService = projectService;
    }
}