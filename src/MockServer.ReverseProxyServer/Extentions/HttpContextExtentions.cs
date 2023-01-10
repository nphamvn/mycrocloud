using System.Dynamic;
using System.Text;
using System.Text.Json;

namespace MockServer.ReverseProxyServer.Extentions;

public static class HttpContextExtentions
{
    public static async Task WriteResponse(this HttpContext context, HttpResponseMessage response)
    {
        context.Response.StatusCode = (int)response.StatusCode;
        var json = await response.Content.ReadAsStringAsync();
        var data = Encoding.UTF8.GetBytes(json);
        context.Response.Headers["Content-Length"] = data.Length.ToString();
        await context.Response.Body.WriteAsync(data, 0, data.Length);
    }

    public static async Task<Dictionary<string, object>> GetRequestDictionary(HttpContext context)
    {
        var request = context.Request;
        context.Request.EnableBuffering();
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;
        return new Dictionary<string, object>()
            {
                { "method", request.Method},
                { "path", request.Path.Value},
                { "host", request.Host.Host},
                { "headers", request.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault())},
                { "routeValues", request.RouteValues.ToDictionary(rv => rv.Key, rv => rv.Value)},
                { "query", request.Query.ToDictionary(q=> q.Key, q => q.Value.FirstOrDefault())},
                { "body", !string.IsNullOrEmpty(body)? JsonSerializer.Deserialize<ExpandoObject>(body) : null }
            };
    }
}