namespace Ocelot.WebApplicationFinder.Middleware
{
    using Microsoft.AspNetCore.Builder;

    public static class DownstreamWebApplicationFinderMiddlewareExtensions
    {
        public static IApplicationBuilder UseDownstreamWebApplicationFinderMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DownstreamWebApplicationFinderMiddleware>();
        }
    }
}