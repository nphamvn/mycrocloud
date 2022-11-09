using MockServer.Web.Models;

namespace MockServer.Web.Services;

public class WorkspacesService : IWorkspacesService
{
    public Task AddMockRequest(Workspace workspace, Request request)
    {
        throw new NotImplementedException();
    }

    public Task<Workspace> CreateWorkspaceAsync()
    {
        throw new NotImplementedException();
    }

    public Task DeleteWorkspace()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Workspace>> GetWorkspacesAsync()
    {
        throw new NotImplementedException();
    }
}