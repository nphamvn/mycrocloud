using Microsoft.AspNetCore.Identity;
using MycroCloud.WebMvc.Extentions;

namespace MycroCloud.WebMvc.Areas.Services.Services;

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
