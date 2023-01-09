using MockServer.Core.Enums;

namespace MockServer.ReverseProxyServer.Interfaces;

public interface IRequestHandlerFactory
{
    IRequestHandler GetInstance(RequestType type);
}