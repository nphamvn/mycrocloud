using Microsoft.AspNetCore.Identity;

namespace MycroCloud.WebMvc.Areas.Services.Services;

public class ServiceBaseService
{
    protected IdentityUser ServiceOwner;
    private readonly IHttpContextAccessor _contextAccessor;
    public ServiceBaseService(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
        ServiceOwner = _contextAccessor.HttpContext.Items["ServiceOwner"] as IdentityUser;
    }
}
