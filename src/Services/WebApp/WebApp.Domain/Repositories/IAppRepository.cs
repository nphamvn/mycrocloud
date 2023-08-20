namespace WebApp.Domain.Repositories;
using Entities;
public interface IAppRepository
{
    Task<AppEntity> GetByAppId(int id);
    Task<AppEntity> FindByUserIdAndAppName(string userId, string name);
    Task<IEnumerable<AppEntity>> ListByUserId(string userId, string query, string sort);
    Task Add(string userId, AppEntity app);
    Task Update(int id, AppEntity app);
    Task Delete(int appId);
}