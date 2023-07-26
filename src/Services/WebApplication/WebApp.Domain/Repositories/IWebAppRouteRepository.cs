using WebApp.Domain.Entities;

namespace WebApp.Domain.Repositories;

public interface IWebAppRouteRepository
{
    Task<int> Create(int appId, Entities.RouteEntity route);
    Task<RouteEntity> Find(int appId, string method, string path);
    Task<RouteEntity> Get(int id);
    Task<IEnumerable<RouteEntity>> GetByApplicationId(int appId, string searchTerm, string sort);
    Task Update(int id, RouteEntity route);
    Task Delete(int id);
    Task<RouteMockResponse> GetMockResponse(int id);
}