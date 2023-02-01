using AutoMapper;
using MockServer.Core.Models.Auth;
using MockServer.Web.Models.Requests;

namespace MockServer.Web.Profiles;

public class RequestProfile : Profile
{
    public RequestProfile()
    {
        CreateMap<Core.Models.Requests.Request, RequestIndexItem>();
        CreateMap<CreateUpdateRequestViewModel, Core.Models.Requests.Request>().ReverseMap();
        CreateMap<Core.Models.Requests.Request, RequestOpenViewModel>().ReverseMap();
        CreateMap<Core.Models.Requests.FixedRequest, FixedRequestConfigViewModel>().ReverseMap();
        CreateMap<AppAuthorization, AuthorizationConfigViewModel>().ReverseMap();
    }
}
