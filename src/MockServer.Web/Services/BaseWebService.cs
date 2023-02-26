using MockServer.Core.Identity;
using MockServer.Web.Extentions;

namespace MockServer.Web.Services;

public class BaseWebService
{
    protected User AuthUser;
    private readonly IHttpContextAccessor _contextAccessor;
    public BaseWebService(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
        AuthUser = _contextAccessor.HttpContext.User.Parse<User>();
    }
}
