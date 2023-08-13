using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MycroCloud.WebMvc.Controllers;
using MycroCloud.WebMvc.Extentions;
using MycroCloud.WebMvc.Identity;
using MycroCloud.WeMvc;

namespace MycroCloud.WebMvc.Areas.Services.Controllers;

[Area(Constants.Area.Services)]
[Route("{Username}/[area]/[controller]")]
[Route("[area]/[controller]")]
public class BaseServiceController : BaseController
{
    public const string ServiceOwnerItem = "ServiceOwner";
    protected MycroCloudIdentityUser ServiceOwner;

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var username = context.HttpContext.Request.RouteValues["Username"]?.ToString();
        var authenticatedUser = context.HttpContext.User?.ToMycroCloudUser();
        if (authenticatedUser is null && username is null)
        {
            context.Result = new NotFoundResult();
            return;
        }
        if (username is null)
        {
            ServiceOwner = authenticatedUser;
        }
        else
        {
            var userManager = context.HttpContext.RequestServices.GetService<UserManager<MycroCloudIdentityUser>>();
            ServiceOwner = await userManager.FindByNameAsync(username);
        }

        await next();
    }
}