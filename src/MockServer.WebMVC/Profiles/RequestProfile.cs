using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MockServer.WebMVC.Models.Request;

namespace MockServer.WebMVC.Profiles;

public class RequestProfile : Profile
{
    public RequestProfile()
    {
        CreateMap<Core.Entities.Request, RequestItem>().ReverseMap();
        //Core.Entities.Request
        CreateMap<CreateRequestViewModel, Core.Entities.Request>().ReverseMap();
    }
}
