using Microsoft.AspNetCore.Identity;
using MockServer.Domain.Identity;
using MockServer.Web.Extentions;

namespace MockServer.Web.Services;

public class BaseService
{
    protected IdentityUser AuthUser;
    private readonly IHttpContextAccessor _contextAccessor;
    public BaseService(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
        AuthUser = _contextAccessor.HttpContext.User.ToIdentityUser();
    }
}
