using System.Text.Json;
using Jint;
using Route = WebApp.Domain.Entities.Route;

namespace WebApp.MiniApiGateway;

public static class StaticResponseHandler
{
    public static async Task Handle(HttpContext context)
    {
        var route = (Route)context.Items["_Route"]!;
        var scripts = context.RequestServices.GetRequiredService<ScriptCollection>();
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
            var request = await ReadRequest(context.Request);
            body = new Engine()
                .SetValue("requestJson", JsonSerializer.Serialize(request))
                .SetValue("source", body)
                .Execute(scripts["handlebars"])
                .Execute("Handlebars.registerHelper('json', function(context) { return JSON.stringify(context); });")
                .Evaluate("""
                    const request = JSON.parse(requestJson);
                    const template = Handlebars.compile(source);
                    template({ request });
                    """)
                .AsString();
        }

        await context.Response.WriteAsync(body);
    }

    private static async Task<object> ReadRequest(HttpRequest request)
    {
        request.EnableBuffering();
        var body = await new StreamReader(request.Body).ReadToEndAsync();
        request.Body.Position = 0;

        return new
        {
            method = request.Method,
            path = request.Path.Value,
            @params = request.RouteValues.ToDictionary(x => x.Key, x => x.Value?.ToString()),
            query = request.Query.ToDictionary(x => x.Key, x => x.Value.ToString()),
            headers = request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
            body = !string.IsNullOrEmpty(body) ? JsonSerializer.Deserialize<dynamic>(body) : null
        };
    }
}