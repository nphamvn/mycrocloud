using Microsoft.AspNetCore.Mvc.Filters;
using MockServer.Core.Repositories;
using MockServer.Web.Extentions;

namespace MockServer.Web.Filters;

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
        if (context.ActionArguments.ContainsKey(_appNameKey))
        {
            appName = (string)context.ActionArguments[_appNameKey];
        }
        else if (context.RouteData.Values.ContainsKey(_appNameKey))
        {
            appName = (string)context.RouteData.Values[_appNameKey];
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
