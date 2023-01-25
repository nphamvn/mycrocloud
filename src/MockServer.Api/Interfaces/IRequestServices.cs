using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Interfaces;

public interface IRequestServices
{
    Task<AppRequest> FindRequest(IncomingRequest model);
    Task<AppRequest> GetRequest(IncomingRequest model);
}