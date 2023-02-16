using MockServer.Core.Models.Auth;
using MockServer.Core.Enums;

namespace MockServer.Core.Repositories;

public interface IAuthRepository
{
    Task AddProjectAuthenticationScheme(int projectId, AuthenticationScheme option);
    Task UpdateProjectAuthenticationScheme(int projectId, AuthenticationScheme option);
    Task ActivateProjectAuthenticationSchemes(int projectId, List<int> schemeIds);
    Task<IEnumerable<AuthenticationScheme>> GetProjectAuthenticationSchemes(int projectId);
    Task<AuthenticationScheme> GetAuthenticationScheme(int id);
    Task<AuthenticationScheme> GetAuthenticationScheme(int id, AuthenticationType type);
    Task<AuthenticationScheme> GetAuthenticationScheme<TOptions>(int id) where TOptions : AuthenticationSchemeOptions;
    Task<Authorization> GetRequestAuthorization(int requestId);
    Task UpdateRequestAuthorization(int requestId, Authorization authorization);
}