namespace MockServer.Api.Middlewares;
public class FilterExecution : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context.Items["RequestId"]);
        var requestId = Convert.ToInt32(context.Items["RequestId"]);
    }
}
public static class FilterExecutionExtensions
{
    public static IApplicationBuilder UseFilterExecution(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<FilterExecution>();
    }
}