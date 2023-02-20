using System.Text;
using MockServer.Core.Interfaces;
using MockServer.Core.Services;
using MockServer.Api.Constraints;

namespace MockServer.Api.Extentions;

public static class ApplicationBuilderExtentions
{
    public static IApplicationBuilder MapTestPaths(this IApplicationBuilder app)
    {
        app.Map("/dev/db/read", app =>
        {
            app.Run(async context =>
            {
                context.Items["Username"] = "npham";
                var handlerContext = new HandlerContext(context);
                handlerContext.Setup();

                var script =
                    """
                    const db = connectDb('tiny_blog');
                    //read data
                    let data = db.read();
                    log(data.posts[0]);
                    """;
                if (!string.IsNullOrEmpty(script))
                {
                    handlerContext.JintEngine.Execute(script);
                }
                IHandlebarsTemplateRenderer renderService = new HandlebarsTemplateRenderer(handlerContext.JintEngine);
                var template =
                    """
                    hello, world
                    """;
                string body = renderService.Render(template);
            });
        });

        app.Map("/dev/db/write", app =>
        {
            app.Run(async context =>
            {
                context.Items["Username"] = "npham";
                var handlerContext = new HandlerContext(context);
                handlerContext.Setup();

                var script =
                    """
                    const db = connectDb('tiny_blog');
                    //read data
                    let data = db.read();
                    //data = { posts: [] };  
                    data.posts.push({title:'hello world'});
                    //write data
                    db.write(data);
                    """;
                if (!string.IsNullOrEmpty(script))
                {
                    handlerContext.JintEngine.Execute(script);
                }
                IHandlebarsTemplateRenderer renderService = new HandlebarsTemplateRenderer(handlerContext.JintEngine);
                var template =
                    """
                    hello, world
                    """;
                string body = renderService.Render(template);
            });
        });

        app.Map("/dev/header-binder", app =>
        {
            app.Run(async context =>
            {
                var binderProvider = (DataBinderProvider)context.RequestServices.GetService(typeof(DataBinderProvider));
                string key = "header(Name)";
                var binder = binderProvider.GetBinder(key);
                var value = binder.Get(context);
                var name = value.ToString();
                await context.Response.WriteAsync(name ?? "Name not found");
            });
        });

        app.Map("/dev/resolve-constraint", app =>
        {
            app.Run(async context =>
            {
                var map = ConstraintBuilder.GetDefaultConstraintMap();
                var builder = new ConstraintBuilder(map);
                builder.AddResolvedConstraint("int");
                builder.AddResolvedConstraint("length(5)");
                builder.Build();
            });
        });

        app.Map("/dev/print-request", app =>
        {
            app.Run(async context =>
            {
                var request = await HttpContextExtentions.GetRequestDictionary(context);
                await context.Response.WriteAsJsonAsync(new
                {
                    request = request
                });
            });
        });

        app.Map("/dev/render-handlebars-template", app =>
        {
            app.Run(async context =>
            {
                var request = await HttpContextExtentions.GetRequestDictionary(context);
                IHandlebarsTemplateRenderer renderService = new HandlebarsTemplateRenderer();
                var ctx = new
                {
                    request = request
                };
                string template =
                        """
                        {
                            "message": "the sum of @{a} and @{b} is @{add(a, b)}"
                        }
                        """;
                string script =
                        """
                        const a = parseInt(ctx.request.query.a);
                        const b = parseInt(ctx.request.query.b);
                        const add = function(a, b) {
                            return a + b;
                        }
                        """;
                var result = renderService.Render(template);
                await context.Response.WriteAsync(result);
            });
        });

        app.Map("/dev/render-expression-template", app =>
        {
            app.Run(async context =>
            {
                var request = await HttpContextExtentions.GetRequestDictionary(context);
                IExpressionTemplateWithScriptRenderer renderService = new ExpressionTemplateWithScriptRenderer();
                var ctx = new
                {
                    request = request
                };
                string template =
                        """
                        {
                            "message": "the sum of @{a} and @{b} is @{add(a, b)}"
                        }
                        """;
                string script =
                        """
                        const a = parseInt(ctx.request.query.a);
                        const b = parseInt(ctx.request.query.b);
                        const add = function(a, b) {
                            return a + b;
                        }
                        """;
                var result = renderService.Render(template);
                context.Response.StatusCode = 200;
                var data = Encoding.UTF8.GetBytes(result);
                context.Response.Headers["content-type"] = "application/json; charset=utf-8";
                await context.Response.Body.WriteAsync(data, 0, data.Length);
            });
        });
        return app;
    }
}
