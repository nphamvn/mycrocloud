using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Interfaces;

public interface IRequestServices
{
    Task<AppRequest> FindRequest(RequestModel model);
    Task<AppRequest> GetRequest(RequestModel model);
}