using MockServer.Core.Models.Requests;

namespace MockServer.Core.Repositories;

public interface IRequestRepository
{
    Task<int> Create(int projectId, Request request);
    Task<Request> Find(int projectId, string method, string path);
    Task<Request> GetById(int id);
    Task<IEnumerable<Request>> GetByProjectId(int ProjectId);
    Task Update(int id, Request request);
    Task Delete(int id);
    Task UpdateRequestParams(int id, IList<RequestParam> parameters);
    Task UpdateRequestHeaders(int id, IList<RequestHeader> parameters);
    Task UpdateRequestBody(int id, FixedRequest config);
    Task<ForwardingRequest> GetForwardingRequest(int requestId);
    Task UpdateResponseHeaders(int id, FixedRequest config);
    Task UpdateResponse(int id, FixedRequest config);
    Task<RequestBody> GetRequestBody(int requestId);
    Task<IEnumerable<ResponseHeader>> GetResponseHeaders(int id);
    Task<Response> GetResponse(int requestId);
}