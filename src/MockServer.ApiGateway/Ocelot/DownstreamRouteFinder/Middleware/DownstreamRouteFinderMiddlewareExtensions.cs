namespace Ocelot.DownstreamRouteFinder.Middleware
{
    using Microsoft.AspNetCore.Builder;

    public static class RouteFinderMiddlewareExtensions
    {
        public static IApplicationBuilder UseRouteFinderMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RouteFinderMiddleware>();
        }
    }
}
