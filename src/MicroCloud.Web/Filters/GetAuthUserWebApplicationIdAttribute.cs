using Microsoft.AspNetCore.Mvc.Filters;
using WebApplication.Domain.Repositories;
using MicroCloud.Web.Extentions;

namespace MicroCloud.Web.Filters;

public class GetAuthUserWebApplicationIdAttribute : ActionFilterAttribute
{
    private readonly string _appNameKey;
    private readonly string _setKey;
    public GetAuthUserWebApplicationIdAttribute(string appNameKey, string setKey)
    {
        _appNameKey = appNameKey;
        _setKey = setKey;
    }
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User.ToIdentityUser();
        var webApplicationRepository = context.HttpContext.RequestServices.GetService<IWebApplicationRepository>();
        string appName = null;
        if (context.ActionArguments.TryGetValue(_appNameKey, out var argument))
        {
            appName = (string)argument;
        }
        else if (context.RouteData.Values.TryGetValue(_appNameKey, out var value))
        {
            appName = (string)value;
        }
        if (!string.IsNullOrEmpty(appName))
        {
            var app = await webApplicationRepository.FindByUserId(user.Id, appName);
            if (app != null)
            {
                context.ActionArguments[_setKey] = app.WebApplicationId;
            }
        }
        await base.OnActionExecutionAsync(context, next);
    }
}
