using MockServer.Core.Models.Auth;
using MockServer.Core.Models.Requests;

namespace MockServer.Core.Repositories;

public interface IRequestRepository
{
    Task<int> Create(int projectId, Request request);
    Task<Request> Find(int projectId, string method, string path);
    Task<Request> GetById(int id);
    Task<IEnumerable<Request>> GetByProjectId(int projectId);
    Task Update(int id, Request request);
    Task Delete(int id);
    Task<IEnumerable<RequestQuery>> GetRequestQueries(int id);
    Task<IEnumerable<RequestHeader>> GetRequestHeaders(int id);
    Task<RequestBody> GetRequestBody(int id);
    Task<IEnumerable<ResponseHeader>> GetResponseHeaders(int id);
    Task<Response> GetResponse(int id);
    Task UpdateRequestQuery(int id, IList<RequestQuery> queries);
    Task UpdateRequestHeader(int id, IList<RequestHeader> headers);
    Task UpdateRequestBody(int id, RequestBody body);
    Task<ForwardingRequest> GetForwardingRequest(int requestId);
    Task UpdateResponseHeaders(int id, FixedRequest config);
    Task UpdateResponse(int id, FixedRequest config);
    Task AttachAuthorization(int id, Authorization authorization);
    Task<Authorization> GetAuthorization(int id);
}