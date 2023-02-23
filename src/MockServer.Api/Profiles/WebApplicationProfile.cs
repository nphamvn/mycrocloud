using AutoMapper;
using MockServer.Core.Models.Projects;
using WebApplication = MockServer.Api.TinyFramework.WebApplication;

namespace MockServer.Api.Profiles;

public class WebApplicationProfile: Profile
{
    public WebApplicationProfile()
    {
        CreateMap<Project, WebApplication>();
    }
}
