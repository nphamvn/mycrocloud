using AutoMapper;
using MockServer.Web.Models.Projects;
using CoreProject = MockServer.Core.Models.Projects.Project;
namespace MockServer.Web.Profiles;

public class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        CreateMap<CreateProjectViewModel, CoreProject>().ReverseMap();
        CreateMap<CoreProject, ProjectIndexItem>().ReverseMap();
        CreateMap<CoreProject, Project>().ReverseMap();
    }
}
