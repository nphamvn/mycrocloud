using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.Infrastructure.Repositories.EfCore;
using WebApp.RestApi.Extensions;

namespace WebApp.RestApi.Filters;

public class AppOwnerActionFilter(AppDbContext appDbContext,
    ILogger<AppOwnerActionFilter> logger, string appIdArgumentName = "appId")
    : IAsyncActionFilter
{
    private bool TryGetAppId(ActionExecutingContext context, out int? appId)
    {
        appId = null;
        
        // Get from RouteData, ActionArguments.
        if (context.RouteData.Values.TryGetValue(appIdArgumentName, out var routeValue))
        {
            appId = int.Parse(routeValue!.ToString() ?? throw new InvalidOperationException());
            return true;
        }
        
        if (context.ActionArguments.TryGetValue(appIdArgumentName, out var actionValue))
        {
            appId = int.Parse(actionValue!.ToString() ?? throw new InvalidOperationException());
            return true;
        }
        
        return false;
    }
    
    public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        logger.LogDebug("Executing AppOwnerActionFilter");
        if (context.HttpContext.User.Identity is null || !context.HttpContext.User.Identity.IsAuthenticated)
        {
            logger.LogWarning("User is not authenticated");
            return next();
        }
        var userId = context.HttpContext.User.GetUserId();
        logger.LogDebug("UserId: {UserId}", userId);
        
        if (!TryGetAppId(context, out var appId))
        {
            logger.LogWarning("AppId argument is missing");
            return next();
        }
        
        logger.LogDebug("AppId: {AppId}", appId);

        var app = appDbContext.Apps.Find(appId);
        var isAppOwner = app?.UserId == userId;
        logger.LogDebug("IsAppOwner: {IsAppOwner}", isAppOwner);
        if (!isAppOwner)
        {
            logger.LogWarning("User {UserId} is not the owner of the app {AppId}", userId, appId);
            context.Result = new ForbidResult();
            return Task.CompletedTask;
        }
        
        logger.LogDebug("User {UserId} is the owner of the app {AppId}", userId, appId);
        context.HttpContext.Items["App"] = app!;
        return next();
    }
}