using MockServer.Core.Interfaces;
using MockServer.Core.Services;

namespace MockServer.ReverseProxyServer.Extentions;

public static class ApplicationBuilderExtentions
{
    public static IApplicationBuilder MapTestPaths(this IApplicationBuilder app)
    {
        app.Map("/test/print-request", app =>
        {
            app.Run(async context =>
            {
                var request = HttpContextExtentions.GetRequestDictionary(context);
                await context.Response.WriteAsJsonAsync(request);
            });
        });

        app.Map("/test/render-handlebars-template", app =>
        {
            app.Run(async context =>
            {
                var request = HttpContextExtentions.GetRequestDictionary(context);
                ITemplateRenderService renderService = new HandlebarsTemplateRenderService();
                var data = new
                {
                    ctx = new
                    {
                        request = request
                    }
                };
                var result = renderService.Render(data, context.Request.Headers["template"]);
                await context.Response.WriteAsync(result);
            });
        });
        return app;
    }
}
