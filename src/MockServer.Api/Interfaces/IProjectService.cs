using MockServer.Core.Models.Projects;

namespace MockServer.Core.Interfaces;

public interface IProjectService
{
    Task<WebApp> GetProject(int id);
}