using MockServer.Core.Models.Requests;

namespace MockServer.Core.Repositories;

public interface IRequestRepository
{
    Task<Request> Get(int userId, string projectName, int id);
    Task<Request> Get(string username, string projectName, string method, string path);
    Task<Request> Get(int userId, string projectName, string method, string path);
    Task<Request> Get(int projectId, string method, string path);
    Task<Request> Get(int id);
    Task<ForwardingRequest> GetForwardingRequest(int requestId);
    Task<IEnumerable<Request>> GetProjectRequests(int ProjectId);
    Task<int> Create(int userId, string projectName, Request request);
    Task Update(int userId, string projectName, int id, Request request);
    Task UpdateRequestParams(int id, FixedRequest config);
    Task UpdateRequestHeaders(int id, FixedRequest config);
    Task UpdateRequestBody(int id, FixedRequest config);
    Task UpdateResponseHeaders(int id, FixedRequest config);
    Task UpdateResponse(int id, FixedRequest config);
    Task Delete(int userId, string projectName, int id);
    Task<IEnumerable<RequestParam>> GetRequestParams(int id);
    Task<IEnumerable<RequestHeader>> GetRequestHeaders(int id);
    Task<RequestBody> GetRequestBody(int requestId);
    Task<IEnumerable<ResponseHeader>> GetResponseHeaders(int id);
    Task<Response> GetResponse(int requestId);
}