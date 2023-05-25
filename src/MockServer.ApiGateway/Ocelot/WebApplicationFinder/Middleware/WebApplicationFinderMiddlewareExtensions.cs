namespace Ocelot.WebApplicationFinder.Middleware
{
    using Microsoft.AspNetCore.Builder;

    public static class WebApplicationFinderMiddlewareExtensions
    {
        public static IApplicationBuilder UseWebApplicationFinderMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebApplicationFinderMiddleware>();
        }
    }
}