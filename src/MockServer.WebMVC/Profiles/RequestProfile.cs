using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MockServer.WebMVC.Models.Project;
using MockServer.WebMVC.Models.Request;

namespace MockServer.WebMVC.Profiles;

public class RequestProfile : Profile
{
    public RequestProfile()
    {
        CreateMap<Core.Entities.Requests.Request, RequestItem>().ReverseMap();
        CreateMap<CreateUpdateRequestModel, Core.Entities.Requests.Request>().ReverseMap();
        CreateMap<Core.Entities.Requests.Request, RequestOpenViewModel>().ReverseMap();
        CreateMap<Core.Entities.Requests.FixedRequest, FixedRequestConfigViewModel>().ReverseMap();
    }
}
