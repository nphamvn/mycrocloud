using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;
using Route = WebApp.Domain.Entities.Route;

namespace WebApp.MiniApiGateway;

public class LoggingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, ILogRepository logRepository)
    {
        await next.Invoke(context);
        if (context.Items["_App"] is App app)
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
                FunctionExecutionDuration = functionExecutionResult?.Duration
            });
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