using WebApp.Domain.Entities;

namespace WebApp.Domain.Repositories;

public interface IRouteRepository
{
    Task<IEnumerable<RouteEntity>> List(int appId, string searchTerm, string sort);
    Task<int> Add(int appId, RouteEntity route);
    Task<RouteEntity> Find(int appId, string method, string path);
    Task<RouteEntity> GetById(int id);
    Task Update(int id, RouteEntity route);
    Task Delete(int id);
    Task<RouteMockResponse> GetMockResponse(int routeId);
    Task AddMatchMethods(int id, List<string> list);
}