using MockServer.Core.Models.Projects;

namespace MockServer.Core.Repositories;

public interface IProjectRepository
{
    Task<Project> Get(int projectId);
    Task<Project> Find(int userId, string name);
    Task<Project> Find(string username, string name);
    Task<IEnumerable<Project>> Search(int userId, string query, int accessibility, string sort);
    Task Add(Project project);
    Task Update(Project project);
    Task Delete(int id);
}