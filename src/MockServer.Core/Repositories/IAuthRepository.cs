using MockServer.Core.Entities.Auth;
using MockServer.Core.Enums;

namespace MockServer.Core.Repositories;

public interface IAuthRepository
{
    Task Add(int id, Authentication option);
    Task<Authentication> GetAs(int id, AuthType type);
    Task<IEnumerable<Authentication>> GetByProject(int projectId);
}