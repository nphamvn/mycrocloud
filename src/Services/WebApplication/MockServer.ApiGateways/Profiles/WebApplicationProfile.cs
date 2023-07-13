using AutoMapper;
using CoreWebApplication = MockServer.Core.WebApplications.WebApplication;
using WebApplication = MockServer.Api.TinyFramework.WebApplication;
namespace MockServer.Api.Profiles;

public class WebApplicationProfile: Profile
{
    public WebApplicationProfile()
    {
        CreateMap<CoreWebApplication, WebApplication>();
    }
}
