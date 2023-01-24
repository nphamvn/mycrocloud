using MockServer.Core.Entities.Auth;
using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;

namespace MockServer.Core.Repositories;

public interface IAuthRepository
{
    Task Add(int id, AppAuthentication option);
    Task<AppAuthentication> GetAs(int id, AuthType type);
    Task<IEnumerable<AppAuthentication>> GetByProject(int projectId);
    Task<AppAuthorization> GetRequestAuthorization(int requestId);
}