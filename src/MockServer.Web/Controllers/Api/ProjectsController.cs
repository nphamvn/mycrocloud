using Microsoft.AspNetCore.Mvc;
using MockServer.Web.Models.Common;
using MockServer.Web.Models.Project;
using MockServer.Web.Services.Interfaces;

namespace MockServer.Web.Controllers.Api;

public class ProjectsController : ApiController
{
    private readonly IProjectWebService _projectService;

    public ProjectsController(IProjectWebService projectService)
    {
        _projectService = projectService;
    }
}