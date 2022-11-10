using MockServer.Web.Models;

namespace MockServer.Web.Services;

public class WorkspacesService : IProjectService
{
    public Task AddMockRequest(Project workspace, Request request)
    {
        throw new NotImplementedException();
    }

    public Task<Project> CreateWorkspaceAsync()
    {
        throw new NotImplementedException();
    }

    public Task DeleteWorkspace()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Project>> GetWorkspacesAsync()
    {
        throw new NotImplementedException();
    }
}