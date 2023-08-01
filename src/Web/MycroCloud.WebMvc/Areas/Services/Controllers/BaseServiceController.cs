using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MycroCloud.WebMvc.Controllers;
using MycroCloud.WebMvc.Extentions;

namespace MycroCloud.WebMvc.Areas.Services.Controllers;

[Area("Services")]
[Route("{Username}/[area]/[controller]")]
public class BaseServiceController : BaseController
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var username = context.HttpContext.Request.RouteValues["Username"].ToString();
        var authenticatedUser = context.HttpContext.User?.ToMycroCloudUser();
        IdentityUser serviceOwner;
        if (authenticatedUser?.UserName == username)
        {
            serviceOwner = authenticatedUser;
        }
        else
        {
            var userManager = context.HttpContext.RequestServices.GetService<UserManager<IdentityUser>>();
            var user = await userManager.FindByNameAsync(username);
            serviceOwner = user;
        }
        context.HttpContext.Items["ServiceOwner"] = serviceOwner;
    }
}
