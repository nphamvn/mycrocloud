using AutoMapper;
using MockServer.Core.Entities.Projects;
using MockServer.WebMVC.Models.Project;

namespace MockServer.WebMVC.Profiles;

public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        CreateMap<CreateProjectViewModel, Project>().ReverseMap();
        CreateMap<Project, ProjectIndexItem>().ReverseMap();
    }
}
