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