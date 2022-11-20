using MockServer.Core.Enums;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Interfaces;

public interface IRequestHandlerFactory
{
    IRequestHandler GetInstance(AppRequest request);
    IRequestHandler GetInstance(RequestType type);
}