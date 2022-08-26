using MockServer.Web.Models;

namespace MockServer.Web.Services;

public interface IWorkspacesService
{
    Task<IEnumerable<Workspace>> GetWorkspacesAsync();
    Task<Workspace> CreateWorkspaceAsync();
    Task DeleteWorkspace();
    Task AddMockRequest(Workspace workspace, Request request);
}