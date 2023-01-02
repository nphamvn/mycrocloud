using MockServer.Core.Entities.Requests;
using MockServer.Core.Enums;

namespace MockServer.Core.Repositories;

public interface IRequestRepository
{
    Task<Request> Get(int userId, string projectName, int id);
    Task<Request> FindRequest(string username, string projectName, string method, string path);
    Task<Request> FindRequest(int userId, string projectName, RequestMethod method, string path);
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
    Task<IEnumerable<RequestHeader>> GetRequestHeaders(int id);//GetResponseHeaders
    Task<RequestBody> GetRequestBody(int requestId);
    Task<IEnumerable<ResponseHeader>> GetResponseHeaders(int id);
    Task<Response> GetResponse(int requestId);
}