using Microsoft.AspNetCore.Identity;
using WebApplication.Domain.Identity;
using MicroCloud.Web.Extentions;

namespace MicroCloud.Web.Services;

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
