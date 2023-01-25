using MockServer.Core.Models;

namespace MockServer.Core.Interfaces;

public interface IProjectService
{
    Task<Project> GetProject(int id);
}