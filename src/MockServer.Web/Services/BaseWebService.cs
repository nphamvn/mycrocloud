using MockServer.Core.Models;
using MockServer.WebMVC.Extentions;

namespace MockServer.WebMVC.Services;

public class BaseWebService
{
    protected ApplicationUser AuthUser;
    private readonly IHttpContextAccessor _contextAccessor;
    public BaseWebService(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
        AuthUser = _contextAccessor.HttpContext.User.Parse<ApplicationUser>();
    }
}
