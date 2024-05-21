using Jint;
using WebApp.Infrastructure;

namespace WebApp.MiniApiGateway.Middlewares;

public class StaticResponseMiddleware (RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, AppDbContext dbContext, Scripts scripts)
    {
        var route = (Route)context.Items["_Route"]!;
        context.Response.StatusCode = route.ResponseStatusCode ??
                                      throw new InvalidOperationException("ResponseStatusCode is null");
        if (route.ResponseHeaders is not null)
        {
            foreach (var header in route.ResponseHeaders)
            {
                context.Response.Headers.Append(header.Name, header.Value);
            }
        }

        var body = route.ResponseBody;
        if (route.UseDynamicResponse)
        {
            body = new Engine()
                .SetValue("method", context.Request.Method)
                .SetValue("path", context.Request.Path.Value)
                .SetValue("params", body)
                .Execute(scripts.Handlebars)
                .Execute("Handlebars.registerHelper('json', function(context) { return JSON.stringify(context); });")
                .Evaluate("""
                          const data = {
                              request: {
                                   method: method,
                                   path: path,
                                   params: requestJson.params,
                                   query: requestJson.query,
                                   headers: requestJson.headers,
                                   body: JSON.parse(body)
                              }
                          };
                          Handlebars.compile(source)(data);
                          """)
                .AsString();
        }

        await context.Response.WriteAsync(body);
    }
}

public static class StaticResponseMiddlewareExtensions
{
    public static IApplicationBuilder UseStaticResponseMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<StaticResponseMiddleware>();
    }
}