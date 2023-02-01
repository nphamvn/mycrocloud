using MockServer.Api.Models;

namespace MockServer.Api.Interfaces;

public interface IRequestServices
{
    Task<Request> FindRequest(IncomingRequest model);
    Task<Request> GetRequest(IncomingRequest model);
}