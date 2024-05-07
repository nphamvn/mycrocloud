using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApp.Infrastructure.Repositories.EfCore;
using WebApp.RestApi.Extensions;

namespace WebApp.RestApi.Filters;

public class AppOwnerActionFilter(AppDbContext appDbContext,
    ILogger<AppOwnerActionFilter> logger, string appIdArgumentName = "appId")
    : IAsyncActionFilter
{
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
        
        if (!context.ActionArguments.TryGetValue(appIdArgumentName, out var appIdArgument))
        {
            logger.LogWarning("AppId argument is missing");
            return next();
        }
        
        var appId = (int) appIdArgument!;
        logger.LogDebug("AppId: {AppId}", appId);
        
        var isAppOwner = appDbContext.Apps.Any(a => a.Id == appId && a.UserId == userId);
        logger.LogDebug("IsAppOwner: {IsAppOwner}", isAppOwner);
        if (!isAppOwner)
        {
            logger.LogWarning("User {UserId} is not the owner of the app {AppId}", userId, appId);
            context.Result = new ForbidResult();
            return Task.CompletedTask;
        }
        
        logger.LogDebug("User {UserId} is the owner of the app {AppId}", userId, appId);
        return next();
    }
}