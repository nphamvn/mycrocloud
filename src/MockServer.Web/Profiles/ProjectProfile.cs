using AutoMapper;
using MockServer.Core.Entities.Projects;
using MockServer.Web.Models.Project;

namespace MockServer.Web.Profiles;

public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        CreateMap<CreateProjectViewModel, Project>().ReverseMap();
        CreateMap<Project, ProjectIndexItem>().ReverseMap();
    }
}
