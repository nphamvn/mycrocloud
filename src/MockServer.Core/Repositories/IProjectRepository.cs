using MockServer.Core.Models.Projects;

namespace MockServer.Core.Repositories;

public interface IProjectRepository
{
    Task<WebApp> Get(int projectId);
    Task<WebApp> Find(int userId, string name);
    Task<WebApp> Find(string username, string name);
    Task<IEnumerable<WebApp>> Search(int userId, string query, int accessibility, string sort);
    Task Add(WebApp project);
    Task Update(WebApp project);
    Task Delete(int id);
}