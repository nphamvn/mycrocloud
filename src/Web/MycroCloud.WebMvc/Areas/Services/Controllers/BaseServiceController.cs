using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MycroCloud.WebMvc.Controllers;
using MycroCloud.WebMvc.Extentions;

namespace MycroCloud.WebMvc.Areas.Services.Controllers;

[Area(AreaConstants.Services)]
[Route("{Username}/[area]/[controller]")]
[Route("[area]/[controller]")]
[BaseServiceControllerActionGuard]
public class BaseServiceController : BaseController
{
    protected IdentityUser ServiceOwner;

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var username = context.HttpContext.Request.RouteValues["Username"].ToString();
        var authenticatedUser = context.HttpContext.User?.ToMycroCloudUser();
        if (authenticatedUser?.UserName == username)
        {
            ServiceOwner = authenticatedUser;
        }
        else
        {
            var userManager = context.HttpContext.RequestServices.GetService<UserManager<IdentityUser>>();
            ServiceOwner = await userManager.FindByNameAsync(username);
        }
    }
}

public class BaseServiceControllerActionGuardAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (true)
        {
            return;
        }
        base.OnActionExecuting(context);
    }
}
