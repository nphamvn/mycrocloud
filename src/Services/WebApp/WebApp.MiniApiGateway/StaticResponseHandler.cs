using System.Text.Json;
using Jint;
using Route = WebApp.Domain.Entities.Route;

namespace WebApp.MiniApiGateway;

public static class StaticResponseHandler
{
    public const string ScriptTemplate =
"""
const request = {{
    method: '{0}',
    path: '{1}',
    params: JSON.parse(`{2}`),
    query: JSON.parse(`{3}`),
    headers: JSON.parse(`{4}`),
    body: JSON.parse(`{5}`),
}};
const template = Handlebars.compile(`{6}`);
template({{ request }});
""";

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

        string body = string.Empty;
        if (route.UseDynamicResponse)
        {
            context.Request.EnableBuffering();
            var requestBodyString = await new StreamReader(context.Request.Body).ReadToEndAsync();
            context.Request.Body.Position = 0;

            var script = string.Format(ScriptTemplate,
                    context.Request.Method,
                    context.Request.Path,
                    JsonSerializer.Serialize(context.Request.RouteValues.ToDictionary(x => x.Key, x => x.Value?.ToString())),
                    JsonSerializer.Serialize(context.Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString())),
                    JsonSerializer.Serialize(context.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString())),
                    requestBodyString,
                    route.ResponseBody);

            body = new Engine()
                .Execute(scripts["handlebars"])
                .Execute("Handlebars.registerHelper('json', function(context) { return JSON.stringify(context); });")
                .Evaluate(script)
                .AsString();
        }
        else
        {
            body = route.ResponseBody;
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