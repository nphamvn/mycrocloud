using MockServer.Core.Identity;
using MockServer.Core.Repositories;

namespace MockServer.Infrastructure.Repositories;
public class UserRepository : IUserRepository
{
    public Task<User> FindUserByEmail(string email)
    {
        return Task.FromResult(new User {
            Id = Random.Shared.Next(),
            Email = email
        });
    }
}