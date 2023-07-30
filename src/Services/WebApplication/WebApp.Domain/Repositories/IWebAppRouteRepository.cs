using WebApp.Domain.Entities;

namespace WebApp.Domain.Repositories;

public interface IWebAppRouteRepository
{
    Task<IEnumerable<RouteEntity>> GetAll(string userId, string appName, string searchTerm, string sort);
    Task<int> Create(int appId, RouteEntity route);
    Task<RouteEntity> Find(int appId, string method, string path);
    Task<RouteEntity> Get(string userId, string appName, int routeId);
    Task<IEnumerable<RouteEntity>> GetByApplicationId(int appId, string searchTerm, string sort);
    Task Update(int id, RouteEntity route);
    Task Delete(int id);
    Task<RouteMockResponse> GetMockResponse(int id);
}