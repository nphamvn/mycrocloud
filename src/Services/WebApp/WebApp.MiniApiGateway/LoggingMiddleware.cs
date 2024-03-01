using WebApp.Domain.Entities;
using WebApp.Domain.Enums;
using WebApp.Domain.Repositories;
using Route = WebApp.Domain.Entities.Route;

namespace WebApp.MiniApiGateway;

public class LoggingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, ILogger<LoggingMiddleware> logger, ILogRepository logRepository, IRouteRepository routeRepository)
    {
        foreach (var header in context.Request.Headers)
        {
            logger.LogInformation("Header: {Key}: {Value}", header.Key, header.Value);
        }
        await next.Invoke(context);
        if (context.Items["_App"] is App app && !context.Request.IsPreflightRequest())
        {
            var route = context.Items["_Route"] as Route;
            var functionExecutionResult = context.Items["_FunctionExecutionResult"] as FunctionExecutionResult;
            await logRepository.Add(new Log
            {
                App = app,
                Route = route,
                Method = context.Request.Method,
                Path = context.Request.Path,
                StatusCode = context.Response.StatusCode,
                AdditionalLogMessage = functionExecutionResult?.AdditionalLogMessage,
                FunctionExecutionDuration = functionExecutionResult?.Duration,
                RemoteAddress = context.Request.Headers["CF-Connecting-IP"].ToString()
            });

            if (functionExecutionResult?.Exception is { } e)
            {
                if (e is TimeoutException && route is not null)
                {
                    route.Status = RouteStatus.Blocked;
                    await routeRepository.Update(route.Id, route);
                }
            }
        }
    }
}

public static class LoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LoggingMiddleware>();
    }
}