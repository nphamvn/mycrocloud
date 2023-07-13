namespace WebApplication.Domain.WebApplication.Repositories;
using Entities;
public interface IWebApplicationRepository
{
    Task<WebApplication> Get(int id);
    Task<WebApplication> FindByUserId(string userId, string name);
    Task<WebApplication> FindByUsername(string username, string name);
    Task<IEnumerable<WebApplication>> Search(string userId, string query, string sort);
    Task Add(string userId, WebApplication app);
    Task Update(int id, WebApplication app);
    Task Delete(int id);
}