using MockServer.Core.Interfaces;
using MockServer.Core.Services;

namespace MockServer.ReverseProxyServer.Extentions;

public static class ApplicationBuilderExtentions
{
    public static IApplicationBuilder MapTestPaths(this IApplicationBuilder app)
    {
        app.Map("/dev/print-request", app =>
        {
            app.Run(async context =>
            {
                var request = HttpContextExtentions.GetRequestDictionary(context);
                await context.Response.WriteAsJsonAsync(request);
            });
        });

        app.Map("/dev/render-handlebars-template", app =>
        {
            app.Run(async context =>
            {
                var request = HttpContextExtentions.GetRequestDictionary(context);
                IHandlebarsTemplateRenderer renderService = new HandlebarsTemplateRenderer();
                var ctx = new
                {
                    request = request
                };
                var result = renderService.Render(ctx, context.Request.Headers["template"]);
                await context.Response.WriteAsync(result);
            });
        });

        app.Map("/dev/render-expression-template", app =>
        {
            app.Run(async context =>
            {
                var request = HttpContextExtentions.GetRequestDictionary(context);
                IExpressionTemplateWithScriptRenderer renderService = new ExpressionTemplateWithScriptRenderer();
                var ctx = new
                {
                    request = request
                };
                string template =
                        """
                        {
                            "message": "the sum of @{number1} and @{number2} is @{add(number1, number2)}"
                        }
                        """;
                string script =
                        """
                        const number1 = parseInt(ctx.request.headers.number1);
                        const number2 = parseInt(ctx.request.headers.number2);
                        const add = function(a, b) {
                            return a + b;
                        }
                        """;
                var result = renderService.Render(ctx, template, script);
                await context.Response.WriteAsync(result);
            });
        });
        return app;
    }
}
