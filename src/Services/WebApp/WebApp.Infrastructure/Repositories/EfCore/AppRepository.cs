using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;

namespace WebApp.Infrastructure.Repositories.EfCore;

public class AppRepository(AppDbContext dbContext) : IAppRepository
{
    public async Task Add(string userId, App app)
    {
        app.UserId = userId;
        await dbContext.Apps.AddAsync(app);
        await dbContext.SaveChangesAsync();
    }

    public async Task Delete(int appId)
    {
        var app = await dbContext.Apps.FirstAsync(a => a.Id == appId);
        dbContext.Apps.Remove(app);
        await dbContext.SaveChangesAsync();
    }

    public async Task<List<AuthenticationScheme>> GetAuthenticationSchemes(int appId)
    {
        return await dbContext.AuthenticationSchemes
            .Where(a => a.AppId == appId)
            .ToListAsync();
    }

    public async Task<App> FindByAppId(int id)
    {
        return await dbContext.Apps.FirstOrDefaultAsync(a => a.Id == id);
    }

    public Task<App> FindByUserIdAndAppName(string userId, string name)
    {
        throw new NotImplementedException();
    }

    public async Task<App> GetByAppId(int id)
    {
        return await dbContext.Apps.FirstAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<App>> ListByUserId(string userId, string query, string sort)
    {
        return await dbContext.Apps.Where(a => a.UserId == userId).ToListAsync();
    }

    public async Task Update(int id, App app)
    {
        dbContext.Apps.Update(app);
        await dbContext.SaveChangesAsync();
    }
}