using MockServer.Core.Entities;

namespace MockServer.Core.Repositories;

public interface IProjectRepository
{
    Task<Project> GetById(int id);
    Task<Project> Find(int userId, string projectName);
    Task<IEnumerable<Project>> GetByUserId(int userId);
    Task Add(Project project);
    Task Update(Project project);
}