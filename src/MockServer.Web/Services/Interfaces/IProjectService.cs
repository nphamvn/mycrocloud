using MockServer.Core.Enums;
using MockServer.Web.Models.Projects;

namespace MockServer.Web.Services.Interfaces;

public interface IProjectWebService
{
    Task<ProjectIndexViewModel> GetIndexViewModel(ProjectSearchModel searchModel);
    Task<bool> Create(CreateProjectViewModel project);
    Task<ProjectViewViewModel> GetProjectViewViewModel(int projectId);
    Task Rename(int projectId, string newName);
    Task<string> GenerateKey(int projectId);
    Task Delete(int projectId);
    Task SetAccessibility(int projectId, ProjectAccessibility accessibility);
}