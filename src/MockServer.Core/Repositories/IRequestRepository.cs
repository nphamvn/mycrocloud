using MockServer.Core.Entities;
using MockServer.Core.Enums;

namespace MockServer.Core.Repositories;

public interface IRequestRepository
{
    Task<Request> FindRequest(string username, string projectName, string method, string path);
    Task<FixedResponse> GetFixedResponse(int requestId);
    Task<ForwardingRequest> GetForwardingRequest(int requestId);
}