using MockServer.Core.Entities;

namespace MockServer.Core.Repositories;

public interface IProjectRepository
{
    Task<Project> GetById(int id);
    Task<Project> Find(string email, string projectName);
}