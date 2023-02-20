using AutoMapper;
using MockServer.Core.Models.Auth;
using MockServer.Web.Models.ProjectRequests;

namespace MockServer.Web.Profiles;

public class RequestProfile : Profile
{
    public RequestProfile()
    {
        CreateMap<Core.Models.Requests.Request, RequestIndexItem>();
        CreateMap<SaveRequestViewModel, Core.Models.Requests.Request>().ReverseMap();
        CreateMap<Core.Models.Requests.Request, RequestViewModel>().ReverseMap();
        CreateMap<Core.Models.Requests.FixedRequest, RequestConfiguration>().ReverseMap();
        CreateMap<Authorization, AuthorizationConfiguration>().ReverseMap();
    }
}
