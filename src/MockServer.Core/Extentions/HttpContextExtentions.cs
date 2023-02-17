using System.Dynamic;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace MockServer.Core.Extentions;

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
        var bodyText = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;
        var body = !string.IsNullOrEmpty(bodyText) ? new Dictionary<string, object>(JsonSerializer.Deserialize<ExpandoObject>(bodyText)) : null;
        var headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault());
        var query = request.Query.ToDictionary(q => q.Key, q => q.Value.FirstOrDefault());
        var routeValues = request.RouteValues.ToDictionary(rv => rv.Key, rv => rv.Value);
        return new Dictionary<string, object>()
            {
                { "method", request.Method},
                { "path", request.Path.Value},
                { "host", request.Host.Host},
                { "headers", headers},
                { "routeValues", routeValues},
                { "query", query},
                { "body",  body}
            };
    }
}