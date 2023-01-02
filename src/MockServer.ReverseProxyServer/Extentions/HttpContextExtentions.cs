using System.Text;

namespace MockServer.ReverseProxyServer.Extentions;

public static class HttpContextExtentions
{
    public static async Task WriteResponse(this HttpContext context, HttpResponseMessage response)
    {
        context.Response.StatusCode = (int)response.StatusCode;
        var json = await response.Content.ReadAsStringAsync();
        var data = Encoding.UTF8.GetBytes(json);
        await context.Response.Body.WriteAsync(data, 0, data.Length);
    }

    public static Dictionary<string, object> GetRequestDictionary(HttpContext context)
    {
        var request = context.Request;

        return new Dictionary<string, object>()
            {
                { "method", request.Method},
                { "path", request.Path.ToString()},
                { "headers", request.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault())}
            };
    }
}