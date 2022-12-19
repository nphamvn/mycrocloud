using MockServer.Core.Entities;
using MockServer.Core.Entities.Projects;

namespace MockServer.Core.Repositories;

public interface IProjectRepository
{
    Task<Project> GetById(int id);
    Task<Project> Find(int userId, string projectName);
    Task<IEnumerable<Project>> GetByUserId(int userId, string query, int accessibility, string sort);
    Task Add(Project project);
    Task Update(Project project);
    Task Delete(int id);
}