using Microsoft.AspNetCore.Mvc.Filters;
using MycroCloud.WebMvc.Extentions;
using MycroCloud.WebMvc.Areas.Services.Services;

namespace MycroCloud.WebMvc.Filters;

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
        var user = context.HttpContext.User.ToMycroCloudUser();
        var webAppService = context.HttpContext.RequestServices.GetService<IWebAppService>();
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
            var app = await webAppService.Find(user.Id, appName);
            if (app != null)
            {
                context.ActionArguments[_setKey] = app.WebAppId;
            }
        }
        await base.OnActionExecutionAsync(context, next);
    }
}
