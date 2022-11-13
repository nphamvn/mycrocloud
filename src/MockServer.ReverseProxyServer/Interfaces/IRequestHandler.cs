using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Interfaces;

public interface IRequestHandler
{
    Task<AppResponse> Handle(AppRequest request);
}

public interface IRequestHandler<T> : IRequestHandler where T : AppRequest
{

}