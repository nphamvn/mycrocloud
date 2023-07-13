using WebApplication.Domain.WebApplication.Entities;

namespace WebApplication.Domain.WebApplication.Repositories;

public interface IWebApplicationRouteRepository
{
    Task<int> Create(int appId, Entities.Route route);
    Task<Route> Find(int appId, string method, string path);
    Task<Route> Get(int id);
    Task<IEnumerable<Route>> GetByApplicationId(int appId, string searchTerm, string sort);
    Task Update(int id, Route route);
    Task Delete(int id);
    Task<RouteMockResponse> GetMockResponse(int id);
}