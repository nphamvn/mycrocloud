using AutoMapper;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Profiles;

public class RequestProfile : Profile
{
    public RequestProfile()
    {
        CreateMap<MockServer.Core.Entities.Request, AppRequest>();
    }
}