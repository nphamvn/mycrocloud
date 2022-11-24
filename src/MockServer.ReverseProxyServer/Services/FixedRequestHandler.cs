using System.Net;
using MockServer.Core.Repositories;
using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Services;

public class FixedRequestHandler : IRequestHandler
{
    private readonly IRequestRepository _requestRepository;

    public FixedRequestHandler(IRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public async Task<ResponseMessage> GetResponseMessage(AppRequest request)
    {
        var response = await _requestRepository.GetFixedResponse(request.Id);

        return new ResponseMessage
        {
            StatusCode = (HttpStatusCode)response.StatusCode,
            Content = new StringContent(response.Body)
        };
    }
}