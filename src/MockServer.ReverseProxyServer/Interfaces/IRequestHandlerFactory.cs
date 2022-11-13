using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Interfaces;

public interface IRequestHandlerFactory
{
    IRequestHandler GetInstance(AppRequest request);
}