using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Interfaces;

public interface IRequestHandler
{
    Task<ResponseMessage> GetResponseMessage(AppRequest request);
}

public interface IRequestHandler<T> : IRequestHandler where T : AppRequest
{

}