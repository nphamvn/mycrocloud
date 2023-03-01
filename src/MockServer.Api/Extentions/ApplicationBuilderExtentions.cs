using MockServer.Core.Services;

namespace MockServer.Api.Extentions;

public static class ApplicationBuilderExtentions
{
    public static IApplicationBuilder MapTestPaths(this IApplicationBuilder app)
    {
        return app;
    }
}
