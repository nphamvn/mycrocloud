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
            var requestBody = await new StreamReader(context.Request.Body).ReadToEndAsync();
            var requestHeaders = context.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString());
            var requestQuery = context.Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
            var requestParams = context.Request.RouteValues.ToDictionary(x => x.Key, x => x.Value.ToString());
            
            body = new Engine()
                .SetValue("method", context.Request.Method)
                .SetValue("path", context.Request.Path.Value)
                .SetValue("headers", requestHeaders)
                .SetValue("query", requestQuery)
                .SetValue("params", requestParams)
                .SetValue("bodyParser", "json")
                .SetValue("body", requestBody) // pass raw body. we don't parse it here because we don't know the content type. Even if we know, we don't trust the content type.
                .SetValue("source", body)
                .Execute(scripts.Handlebars)
                .Execute("Handlebars.registerHelper('json', function(context) { return JSON.stringify(context); });")
                .Evaluate("""
                          const request = {
                              method: method,
                              path: path,
                              headers: headers,
                              query: query,
                              params: params,
                          }
                          
                          switch (bodyParser) {
                              case 'json':
                                  request.body = body ? JSON.parse(body) : null;
                                  break;
                          }
                          
                          const data = {
                              request
                          }
                          
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