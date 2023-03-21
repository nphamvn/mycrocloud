using MockServer.Core.WebApplications;
using MockServer.Core.WebApplications.Security;

namespace MockServer.Core.Repositories;

public interface IWebApplicationRouteRepository
{
    Task<int> Create(int appId, Route request);
    Task<Route> Find(int appId, string method, string path);
    Task<Route> GetById(int id);
    Task<IEnumerable<Route>> GetByApplicationId(int appId, string searchTerm, string sort);
    Task Update(int id, Route route);
    Task Delete(int id);
    Task AttachAuthorization(int id, Authorization authorization);
    Task<Authorization> GetAuthorization(int id);
    Task<MockIntegration> GetMockIntegration(int id);
    Task UpdateMockIntegration(int id, MockIntegration integration);
    Task<DirectForwardingIntegration> GetDirectForwardingIntegration(int id);
    Task UpdateDirectForwardingIntegration(int id, DirectForwardingIntegration integration);
    Task<FunctionTriggerIntegration> GetFunctionTriggerIntegration(int id);
    Task UpdateFunctionTriggerIntegration(int id, FunctionTriggerIntegration integration);
}