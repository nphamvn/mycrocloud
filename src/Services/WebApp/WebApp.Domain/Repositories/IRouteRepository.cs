using WebApp.Domain.Entities;

namespace WebApp.Domain.Repositories;

public interface IRouteRepository
{
    Task<IEnumerable<Route>> List(int appId, string searchTerm, string sort);
    Task<int> Add(int appId, Route route);
    Task<Route> GetById(int id);
    Task Update(int id, Route route);
    Task Delete(int id);
    Task<List<RouteValidation>> GetValidations(int routeId);
    Task<Route> GetByIdAsNoTracking(int id);
}