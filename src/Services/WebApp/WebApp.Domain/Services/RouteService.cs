using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;

namespace WebApp.Domain.Services;

public interface IRouteService
{
    Task<int> Create(int appId, Route route);
    Task Update(int id, Route route);
}
public class RouteService : IRouteService
{
    private readonly IRouteRepository _routeRepository;

    public RouteService(IRouteRepository routeRepository)
    {
        _routeRepository = routeRepository;
    }
    public async Task<int> Create(int appId, Route route)
    {
        route.AppId = appId;
        return await _routeRepository.Add(appId, route);
    }

    public async Task Update(int id, Route route)
    {
        await _routeRepository.Update(id, route);
    }
}
