using MockServer.Core.WebApplications;

namespace MockServer.Core.Repositories;

public interface IWebApplicationRepository
{
    Task<WebApplication> Get(int id);
    Task<WebApplication> Find(int userId, string name);
    Task<WebApplication> Find(string username, string name);
    Task<IEnumerable<WebApplication>> Search(int userId, string query, int accessibility, string sort);
    Task Add(int userId, WebApplication app);
    Task Update(WebApplication app);
    Task Delete(int id);
}