using MockServer.Core.Entities.Auth;
using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;

namespace MockServer.Core.Repositories;

public interface IAuthRepository
{
    Task AddProjectAuthenticationScheme(int projectId, AppAuthentication option);
    Task UpdateProjectAuthenticationScheme(int projectId, AppAuthentication option);
    Task ActivateProjectAuthenticationSchemes(int projectId, List<int> schemeIds);
    Task<IEnumerable<AppAuthentication>> GetProjectAuthenticationSchemes(int projectId);
    Task<AppAuthentication> GetAuthenticationScheme(int id);
    Task<AppAuthentication> GetAuthenticationScheme(int id, AuthenticationType type);
    Task<AppAuthentication> GetAuthenticationScheme<TAuthOptions>(int id) where TAuthOptions : AuthOptions;
    Task<AppAuthorization> GetRequestAuthorization(int requestId);
    Task SetRequestAuthorization(int requestId, AppAuthorization authorization);
}