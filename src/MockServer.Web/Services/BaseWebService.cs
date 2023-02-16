using MockServer.Core.Models;
using MockServer.Core.Models.Projects;
using MockServer.Core.Repositories;
using MockServer.Web.Extentions;

namespace MockServer.Web.Services;

public class BaseWebService
{
    protected ApplicationUser AuthUser;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IProjectRepository _projectRepository;
    public BaseWebService(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
        AuthUser = _contextAccessor.HttpContext.User.Parse<ApplicationUser>();
        _projectRepository = _contextAccessor.HttpContext.RequestServices.GetService<IProjectRepository>();
    }
    protected Task<WebApp> GetProject(string projectName)
    {
        return _projectRepository.Find(AuthUser.Id, projectName);
    }
}
