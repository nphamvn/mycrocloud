using MockServer.Core.Enums;
using MockServer.Web.Models.Projects;

namespace MockServer.Web.Services.Interfaces;

public interface IProjectWebService
{
    Task<Project> Get(int projectId);
    Task<ProjectIndexViewModel> GetIndexViewModel(ProjectSearchModel searchModel);
    Task<bool> Create(CreateProjectViewModel project);
    Task Rename(int projectId, string newName);
    Task<string> GenerateKey(int projectId);
    Task Delete(int projectId);
    Task SetAccessibility(int projectId, ProjectAccessibility accessibility);
}