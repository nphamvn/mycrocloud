using System.Text;

namespace MockServer.ReverseProxyServer.Models;

public class AppResponse
{
    private readonly HttpContext _context;
    private readonly HttpResponseMessage responseMessage;

    public AppResponse(HttpContext context, HttpResponseMessage responseMessage)
    {
        this._context = context;
        this.responseMessage = responseMessage;
    }
    public async Task WriteResponse()
    {
        _context.Response.StatusCode = (int)responseMessage.StatusCode;
        var content = responseMessage.Content;
        var json = await content.ReadAsStringAsync();
        var data = Encoding.UTF8.GetBytes(json);
        await _context.Response.Body.WriteAsync(data, 0, data.Length);
    }
}