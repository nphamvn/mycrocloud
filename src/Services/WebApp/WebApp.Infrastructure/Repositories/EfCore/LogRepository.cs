using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;

namespace WebApp.Infrastructure.Repositories.EfCore;

public class LogRepository(AppDbContext appDbContext) : ILogRepository
{
    public async Task Add(Log log)
    {
        await appDbContext.Logs.AddAsync(log);
        await appDbContext.SaveChangesAsync();
    }

    public async Task DeleteByRouteId(int id)
    {
        var logs = appDbContext.Logs.Where(l => l.RouteId == id);
        appDbContext.Logs.RemoveRange(logs);
        await appDbContext.SaveChangesAsync();
    }

    public async Task<IQueryable<Log>> Search(int appId)
    {
        return appDbContext.Logs.Where(l => l.AppId == appId);
    }
}
