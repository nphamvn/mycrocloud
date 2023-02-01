using MockServer.Core.Enums;
using MockServer.Web.Models.Projects;

namespace MockServer.Web.Services.Interfaces;

public interface IProjectWebService
{
    Task<ProjectIndexViewModel> GetIndexViewModel(ProjectSearchModel searchModel);
    Task<bool> Create(CreateProjectViewModel project);
    Task<ProjectViewViewModel> GetProjectViewViewModel(string name);
    Task Rename(string name, string newName);
    Task<string> GenerateKey(string name);
    Task Delete(string name);
    Task SetAccessibility(string name, ProjectAccessibility accessibility);
}