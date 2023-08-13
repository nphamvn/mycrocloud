using Grpc.Core;
using WebApp.Domain.Repositories;

namespace WebApp.Api.Grpc.Services;

public class WebAppAuthenticationService(IWebAppAuthenticationSchemeRepository authenticationSchemeRepository
    , ILogger<WebAppAuthenticationService> logger) : WebAppAuthentication.WebAppAuthenticationBase
{
    private readonly IWebAppAuthenticationSchemeRepository _authenticationSchemeRepository = authenticationSchemeRepository;
    private readonly ILogger<WebAppAuthenticationService> _logger = logger;

    public override async Task<GetAllSchemeResponse> GetAll(GetAllSchemeRequest request, ServerCallContext context)
    {
        var schemes = await _authenticationSchemeRepository.GetAll(request.UserId, request.AppName);
        var res = new GetAllSchemeResponse();
        res.Schemes.AddRange(schemes.Select(s => new GetAllSchemeResponse.Types.Scheme
        {
            Id = s.SchemeId,
            Type = (GetAllSchemeResponse.Types.Scheme.Types.Type)s.Type,
            Name = s.Name,
            DisplayName = s.DisplayName
        }));
        return res;
    }
}
