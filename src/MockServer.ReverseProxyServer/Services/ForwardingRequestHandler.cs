using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Services;

public class ForwardingRequestHandler : IRequestHandler
{
    public async Task<AppResponse> Handle(AppRequest request)
    {
        var message = Map((ForwardingRequest)request);
        using var client = new HttpClient();
        var response = await client.SendAsync(message);

        return new AppResponse(request.HttpContext, response);
    }

    private HttpRequestMessage Map(ForwardingRequest request)
    {
        var httpRequest = request.HttpContext.Request;
        var path = httpRequest.Path.Value.StartsWith("/") ? httpRequest.Path.Value.Remove(0, 1) : httpRequest.Path.Value;
        var message = new HttpRequestMessage();
        //message.Content = request.co;
        //request.Headers.ToList().ForEach(header => message.Headers.Add(header.Key, header.Value));
        message.Method = new HttpMethod(request.HttpContext.Request.Method);//(HttpMethod)Enum.Parse(typeof(HttpMethod), request.Method);
        message.RequestUri = new Uri(string.Format("{0}://{1}/{2}", request.Scheme, request.Host, path));
        return message;
    }
}