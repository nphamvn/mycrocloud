using System.Text;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Extentions;

public static class HttpContextExtentions
{
    public static async Task WriteResponse(this HttpContext context, HttpResponseMessage response)
    {
        context.Response.StatusCode = (int)response.StatusCode;
        var content = response.Content;
        var json = await content.ReadAsStringAsync();
        var data = Encoding.UTF8.GetBytes(json);
        await context.Response.Body.WriteAsync(data, 0, data.Length);
    }
}