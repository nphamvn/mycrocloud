using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;

namespace WebApp.Infrastructure.Repositories.EfCore;

public class LogRepository : ILogRepository
{
    private readonly AppDbContext _appDbContext;

    public LogRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    public async Task Add(Log log)
    {
        await _appDbContext.Logs.AddAsync(log);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<IQueryable<Log>> Search(int appId)
    {
        return _appDbContext.Logs.Where(l => l.AppId == appId);
    }
}
