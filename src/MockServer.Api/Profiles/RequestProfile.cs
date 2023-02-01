using AutoMapper;
using MockServer.Api.Models;

namespace MockServer.Api.Profiles;

public class RequestProfile : Profile
{
    public RequestProfile()
    {
        CreateMap<MockServer.Core.Models.Requests.Request, Request>();
    }
}