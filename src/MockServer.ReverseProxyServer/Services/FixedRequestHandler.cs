using System.Net;
using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Services;

public class FixedRequestHandler : IRequestHandler
{
    public async Task<AppResponse> Handle(AppRequest request)
    {
        var response = GetResponse((FixedRequest)request);
        var message = new HttpResponseMessage();
        message.StatusCode = (HttpStatusCode)response.StatusCode;
        message.Content = new StringContent(response.Body);
        return new AppResponse(request.HttpContext, message);
    }

    private FixedRequestResponse GetResponse(FixedRequest request)
    {
        return new FixedRequestResponse
        {
            StatusCode = 201,
            Body = "some data"
        };
    }
}