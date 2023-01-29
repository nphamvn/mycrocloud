using MockServer.Core.Entities.Auth;
using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;

namespace MockServer.Core.Repositories;

public interface IAuthRepository
{
    Task Add(int projectId, AppAuthentication option);
    Task Update(int id, AppAuthentication option);
    Task SetProjectAuthentication(int projectId, List<int> schemes);
    Task<AppAuthentication> GetAs(int id, AuthenticationType type);
    Task<IEnumerable<AppAuthentication>> GetByProject(int projectId);
    Task<AppAuthorization> GetRequestAuthorization(int requestId);
    Task SetRequestAuthorization(int requestId, AppAuthorization authorization);
}