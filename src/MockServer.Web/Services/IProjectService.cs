using MockServer.Web.Models;

namespace MockServer.Web.Services;

public interface IProjectService
{
    Task<IEnumerable<Project>> GetWorkspacesAsync();
    Task<Project> CreateWorkspaceAsync();
    Task DeleteWorkspace();
    Task AddMockRequest(Project workspace, Request request);
}