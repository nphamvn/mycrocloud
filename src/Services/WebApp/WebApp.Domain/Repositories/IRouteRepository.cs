using WebApp.Domain.Entities;

namespace WebApp.Domain.Repositories;

public interface IRouteRepository
{
    Task<IEnumerable<Route>> List(int appId, string searchTerm, string sort);
    Task<int> Add(int appId, Route route);
    Task<Route> Find(int appId, string method, string path);
    Task<Route> GetById(int id);
    Task Update(int id, Route route);
    Task Delete(int id);
    Task<RouteMockResponse> GetMockResponse(int routeId);
    Task AddMatchMethods(int id, List<string> list);
    Task<List<RouteValidation>> GetValidations(int routeId);
}