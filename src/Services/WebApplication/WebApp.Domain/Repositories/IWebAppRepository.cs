namespace WebApp.Domain.Repositories;
using Entities;
public interface IWebAppRepository
{
    Task<WebAppEntity> Get(int id);
    Task<WebAppEntity> FindByUserId(int userId, string name);
    Task<WebAppEntity> FindByUsername(string username, string name);
    Task<IEnumerable<WebAppEntity>> Search(string userId, string query, string sort);
    Task Add(int userId, WebAppEntity app);
    Task Update(int id, WebAppEntity app);
    Task Delete(int id);
}