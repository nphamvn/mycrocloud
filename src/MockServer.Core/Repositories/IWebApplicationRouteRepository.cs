using MockServer.Core.WebApplications;
using MockServer.Core.WebApplications.Security;

namespace MockServer.Core.Repositories;

public interface IWebApplicationRouteRepository
{
    Task<int> Create(int appId, Route request);
    Task<Route> Find(int appId, string method, string path);
    Task<Route> GetById(int id);
    Task<IEnumerable<Route>> GetByApplicationId(int appId);
    Task Update(int id, Route route);
    Task Delete(int id);
    Task AttachAuthorization(int id, Authorization authorization);
    Task<Authorization> GetAuthorization(int id);
    Task<IEnumerable<RouteRequestHeader>> GetRequestHeaders(int id);
    Task UpdateRequestHeader(int id, IList<RouteRequestHeader> headers);
    Task<IEnumerable<RouteRequestQuery>> GetRequestQueries(int id);
    Task UpdateRequestQuery(int id, IList<RouteRequestQuery> queries);
    Task<RouteRequestBody> GetRequestBody(int id);
    Task UpdateRequestBody(int id, RouteRequestBody body);
    Task<MockIntegration> GetMockIntegration(int id);
    Task UpdateMockIntegration(int id, MockIntegration integration);
    Task<DirectForwardingIntegration> GetDirectForwardingIntegration(int id);
    Task UpdateDirectForwardingIntegration(int id, DirectForwardingIntegration integration);
    Task<FunctionTriggerIntegration> GetFunctionTriggerIntegration(int id);
    Task UpdateFunctionTriggerIntegration(int id, FunctionTriggerIntegration integration);
}