using AutoMapper;
using MockServer.Core.Models.Auth;
using MockServer.Web.Models.Project;
using MockServer.Web.Models.Request;

namespace MockServer.Web.Profiles;

public class RequestProfile : Profile
{
    public RequestProfile()
    {
        CreateMap<Core.Entities.Requests.Request, RequestItem>().ReverseMap();
        CreateMap<CreateUpdateRequestViewModel, Core.Entities.Requests.Request>().ReverseMap();
        CreateMap<Core.Entities.Requests.Request, RequestOpenViewModel>().ReverseMap();
        CreateMap<Core.Entities.Requests.FixedRequest, FixedRequestConfigViewModel>().ReverseMap();
        CreateMap<AppAuthorization, AuthorizationConfigViewModel>().ReverseMap();
    }
}
