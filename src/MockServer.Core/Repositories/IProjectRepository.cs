using MockServer.Core.Entities.Projects;
using MockServer.Core.Models.Authorization;

namespace MockServer.Core.Repositories;

public interface IProjectRepository
{
    Task<Project> Get(int id);
    Task<Project> Get(int userId, string name);
    Task<Project> Get(string username, string name);
    Task<IEnumerable<Project>> GetByUserId(int userId, string query, int accessibility, string sort);
    Task Add(Project project);
    Task Update(Project project);
    Task Delete(int id);
    Task<JwtHandlerConfiguration> GetJwtHandlerConfiguration(int id);
}