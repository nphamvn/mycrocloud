using MockServer.Core.Repositories;
using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Services;

public class ForwardingRequestHandler : IRequestHandler
{
    private readonly IRequestRepository _requestRepository;

    public ForwardingRequestHandler(IRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }
    public async Task<ResponseMessage> GetResponseMessage(AppRequest request)
    {
        var req = await _requestRepository.GetForwardingRequest(request.Id);
        var message = new HttpRequestMessage();
        var path = request.Path;
        message.Method = new HttpMethod(request.HttpContext.Request.Method);//(HttpMethod)Enum.Parse(typeof(HttpMethod), request.Method);
        message.RequestUri = new Uri(string.Format("{0}://{1}/{2}", req.Scheme, req.Host, path));

        using var client = new HttpClient();
        var response = await client.SendAsync(message);

        return new ResponseMessage
        {
            StatusCode = response.StatusCode,
            Content = response.Content
        };
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