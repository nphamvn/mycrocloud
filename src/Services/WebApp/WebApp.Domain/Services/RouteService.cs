using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;

namespace WebApp.Domain.Services;

public interface IRouteService
{
    Task<int> Create(int appId, Route route);
    Task Delete(int id);
    Task Update(int id, Route route);
}
public class RouteService(IRouteRepository routeRepository, ILogRepository logRepository) : IRouteService
{
    public async Task<int> Create(int appId, Route route)
    {
        route.AppId = appId;
        return await routeRepository.Add(appId, route);
    }

    public async Task Delete(int id)
    {
        await logRepository.DeleteByRouteId(id);
        await routeRepository.Delete(id);
    }

    public async Task Update(int id, Route route)
    {
        await routeRepository.Update(id, route);
    }
}
