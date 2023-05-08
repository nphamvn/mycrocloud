using MockServer.Core.Identity;

namespace MockServer.Core.Repositories;

public interface IUserRepository
{
    Task<User> FindUserByEmail(string email);
}
