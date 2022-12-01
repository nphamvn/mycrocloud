using MockServer.Core.Entities;
using MockServer.Core.Enums;

namespace MockServer.Core.Repositories;

public interface IRequestRepository
{
    Task<Request> Get(int userId, string projectName, int id);
    Task<Request> FindRequest(string username, string projectName, string method, string path);
    Task<Request> FindRequest(int userId, string projectName, RequestMethod method, string path);
    Task<FixedResponse> GetFixedResponse(int requestId);
    Task<ForwardingRequest> GetForwardingRequest(int requestId);
    Task<IEnumerable<Request>> GetProjectRequests(int ProjectId);
    Task Create(int userId, string projectName, Request request);
}