using MockServer.Core.Services;

namespace MockServer.Api.Extentions;

public static class ApplicationBuilderExtentions
{
    public static IApplicationBuilder MapTestPaths(this IApplicationBuilder app)
    {
        app.Map("/dev/db/tiny_blog/posts/new", app =>
        {
            app.Run(async context =>
            {
                context.Items["Username"] = "nampham";
                JintHandlerContext handlerContext = null;//new HandlerContext(context);
                handlerContext.WebApplication = new()
                {
                    UserId = 1,
                    Id = 8
                };
                handlerContext.Setup();

                var script =
                    """
                    //connect db
                    const adapter = createAdapter('tiny_blog');
                    const db = new Db(adapter);
                    //read data
                    db.read();
                    const id = db.data.posts.length + 1;
                    db.data.posts.push({
                        "id": id,
                        "title": "post " + id
                    });
                    db.write();
                    """;
                if (!string.IsNullOrEmpty(script))
                {
                    handlerContext.JintEngine.Execute(script);
                }
            });
        });

        // app.Map("/dev/db/write/post", app =>
        // {
        //     app.Run(async context =>
        //     {
        //         context.Items["Username"] = "nampham";
        //         var handlerContext = new HandlerContext(context);
        //         handlerContext.Setup();
        //         var post =
        //             """
        //             //connect db
        //             const db = connectDb('tiny_blog');
        //             //read data
        //             let data = read(db);
        //             data = data || { posts: [] };
        //             let count = data.posts.length;
        //             //perform on data
        //             data.posts.push({title:'hello world ' +  (count + 1)});
        //             //save data
        //             db.write(data);
        //             """;
        //         if (!string.IsNullOrEmpty(post))
        //         {
        //             handlerContext.JintEngine.Execute(post);
        //         }
        //         IHandlebarsTemplateRenderer renderService = new HandlebarsTemplateRenderer(handlerContext.JintEngine);
        //         var template =
        //             """
        //             hello, world
        //             """;
        //         string body = renderService.Render(template);
        //     });
        // });

        // app.Map("/dev/db/write/comment", app =>
        // {
        //     app.Run(async context =>
        //     {
        //         context.Items["Username"] = "nampham";
        //         var handlerContext = new HandlerContext(context);
        //         handlerContext.Setup();
        //         var comment =
        //             """
        //             //connect db
        //             const db = connectDb('tiny_blog');
        //             //read data
        //             let data = read(db);
        //             //perform on data
        //             data.comments = data.comments || [];
        //             //save data
        //             data.comments.push({postId: 1, 'comment': 'nice'})
        //             db.write(data);
        //             """;
        //         if (!string.IsNullOrEmpty(comment))
        //         {
        //             handlerContext.JintEngine.Execute(comment);
        //         }
        //         IHandlebarsTemplateRenderer renderService = new HandlebarsTemplateRenderer(handlerContext.JintEngine);
        //         var template =
        //             """
        //             hello, world
        //             """;
        //         string body = renderService.Render(template);
        //     });
        // });

        // app.Map("/dev/header-binder", app =>
        // {
        //     app.Run(async context =>
        //     {
        //         var binderProvider = (DataBinderProvider)context.RequestServices.GetService(typeof(DataBinderProvider));
        //         string key = "header(Name)";
        //         var binder = binderProvider.GetBinder(key);
        //         var value = binder.Get(context);
        //         var name = value.ToString();
        //         await context.Response.WriteAsync(name ?? "Name not found");
        //     });
        // });

        // app.Map("/dev/resolve-constraint", app =>
        // {
        //     app.Run(async context =>
        //     {
        //         var map = ConstraintBuilder.GetDefaultConstraintMap();
        //         var builder = new ConstraintBuilder(map);
        //         builder.AddResolvedConstraint("int");
        //         builder.AddResolvedConstraint("length(5)");
        //         builder.Build();
        //     });
        // });

        // app.Map("/dev/print-request", app =>
        // {
        //     app.Run(async context =>
        //     {
        //         var request = await HttpContextExtentions.GetRequestDictionary(context);
        //         await context.Response.WriteAsJsonAsync(new
        //         {
        //             request = request
        //         });
        //     });
        // });

        // app.Map("/dev/render-handlebars-template", app =>
        // {
        //     app.Run(async context =>
        //     {
        //         var request = await HttpContextExtentions.GetRequestDictionary(context);
        //         IHandlebarsTemplateRenderer renderService = new HandlebarsTemplateRenderer();
        //         var ctx = new
        //         {
        //             request = request
        //         };
        //         string template =
        //                 """
        //                 {
        //                     "message": "the sum of @{a} and @{b} is @{add(a, b)}"
        //                 }
        //                 """;
        //         string script =
        //                 """
        //                 const a = parseInt(ctx.request.query.a);
        //                 const b = parseInt(ctx.request.query.b);
        //                 const add = function(a, b) {
        //                     return a + b;
        //                 }
        //                 """;
        //         var result = renderService.Render(template);
        //         await context.Response.WriteAsync(result);
        //     });
        // });

        // app.Map("/dev/render-expression-template", app =>
        // {
        //     app.Run(async context =>
        //     {
        //         var request = await HttpContextExtentions.GetRequestDictionary(context);
        //         IExpressionTemplateWithScriptRenderer renderService = new ExpressionTemplateWithScriptRenderer();
        //         var ctx = new
        //         {
        //             request = request
        //         };
        //         string template =
        //                 """
        //                 {
        //                     "message": "the sum of @{a} and @{b} is @{add(a, b)}"
        //                 }
        //                 """;
        //         string script =
        //                 """
        //                 const a = parseInt(ctx.request.query.a);
        //                 const b = parseInt(ctx.request.query.b);
        //                 const add = function(a, b) {
        //                     return a + b;
        //                 }
        //                 """;
        //         var result = renderService.Render(template);
        //         context.Response.StatusCode = 200;
        //         var data = Encoding.UTF8.GetBytes(result);
        //         context.Response.Headers["content-type"] = "application/json; charset=utf-8";
        //         await context.Response.Body.WriteAsync(data, 0, data.Length);
        //     });
        // });
        return app;
    }
}
