using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;

namespace WebApp.Infrastructure.Repositories.EfCore;

public class AppRepository(AppDbContext dbContext) : IAppRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task Add(string userId, App app)
    {
        app.UserId = userId;
        await _dbContext.Apps.AddAsync(app);
        await _dbContext.SaveChangesAsync();
    }

    public Task Delete(int appId)
    {
        throw new NotImplementedException();
    }

    public Task<App> FindByUserIdAndAppName(string userId, string name)
    {
        throw new NotImplementedException();
    }

    public async Task<App> GetByAppId(int id)
    {
        return await _dbContext.Apps.FirstAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<App>> ListByUserId(string userId, string query, string sort)
    {
        return await _dbContext.Apps.Where(a => a.UserId == userId).ToListAsync();
    }

    public Task Update(int id, App app)
    {
        throw new NotImplementedException();
    }
}