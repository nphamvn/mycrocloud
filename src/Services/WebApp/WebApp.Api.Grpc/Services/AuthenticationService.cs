using Grpc.Core;
using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;

namespace WebApp.Api.Grpc.Services;

public class AuthenticationService(
    IAuthenticationSchemeRepository authenticationSchemeRepository
    , ILogger<AuthenticationService> logger) : WebAppAuthenticationGrpcService.WebAppAuthenticationGrpcServiceBase
{

    public override async Task<GetAllSchemeResponse> GetAll(GetAllSchemeRequest request, ServerCallContext context)
    {
        var schemes = await authenticationSchemeRepository.GetAllByAppId(request.AppId);
        var res = new GetAllSchemeResponse();
        res.Schemes.AddRange(schemes.Select(s => new Scheme
        {
            SchemeId = s.SchemeId,
            Type = (SchemeType)s.Type,
            Name = s.Name,
            DisplayName = s.DisplayName,
            Description = s.Description
        }));
        return res;
    }
    public override async Task<CreateJwtBearerSchemeResponse> CreateJwtBearerScheme(CreateJwtBearerSchemeRequest request, ServerCallContext context)
    {
        await authenticationSchemeRepository.AddJwtBearerScheme(request.AppId, new JwtBearerAuthenticationSchemeEntity
        {
            Name = request.Name,
            DisplayName = request.DisplayName,
            Description = request.Description,
            Issuer = request.Authority,
            Audience = request.Audience
        });
        return new CreateJwtBearerSchemeResponse();
    }
}
