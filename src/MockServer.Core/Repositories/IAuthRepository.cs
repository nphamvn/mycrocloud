using MockServer.Core.Models.Auth;
using MockServer.Core.Enums;

namespace MockServer.Core.Repositories;

public interface IAuthRepository
{
    Task AddProjectAuthenticationScheme(int projectId, AppAuthentication option);
    Task UpdateProjectAuthenticationScheme(int projectId, AppAuthentication option);
    Task ActivateProjectAuthenticationSchemes(int projectId, List<int> schemeIds);
    Task<IEnumerable<AppAuthentication>> GetProjectAuthenticationSchemes(int projectId);
    Task<AppAuthentication> GetAuthenticationScheme(int id);
    Task<AppAuthentication> GetAuthenticationScheme(int id, AuthenticationType type);
    Task<AppAuthentication> GetAuthenticationScheme<TAuthOptions>(int id) where TAuthOptions : AuthenticationOptions;
    Task<AppAuthorization> GetRequestAuthorization(int requestId);
    Task UpdateRequestAuthorization(int requestId, AppAuthorization authorization);
}