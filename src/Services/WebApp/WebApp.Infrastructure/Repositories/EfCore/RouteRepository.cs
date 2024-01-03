using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;

namespace WebApp.Infrastructure.Repositories.EfCore;

public class RouteRepository : IRouteRepository
{
    private readonly AppDbContext _dbContext;

    public RouteRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<int> Add(int appId, Route route)
    {
        route.AppId = appId;
        await _dbContext.Routes.AddAsync(route);
        await _dbContext.SaveChangesAsync();
        return route.Id;
    }

    public Task AddMatchMethods(int id, List<string> list)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(int id)
    {
        var route = await _dbContext.Routes.FirstOrDefaultAsync(r => r.Id == id);
        _dbContext.Routes.Remove(route);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Route> Find(int appId, string method, string path)
    {
        throw new NotImplementedException();
    }

    public async Task<Route> GetById(int id)
    {
        return await _dbContext.Routes.FirstAsync(r => r.Id == id);
    }

    public Task<RouteMockResponse> GetMockResponse(int routeId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Route>> List(int appId, string searchTerm, string sort)
    {
        return await _dbContext.Routes.Where(r => r.AppId == appId).ToListAsync();
    }

    public async Task Update(int id, Route route)
    {
        _dbContext.Routes.Update(route);
        await _dbContext.SaveChangesAsync();
    }
}
