namespace Ocelot.Requester.Middleware
{
    using Microsoft.AspNetCore.Builder;

    public static class RequestForwardingMiddlewareMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestForwardingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestForwardingMiddleware>();
        }
    }
}
