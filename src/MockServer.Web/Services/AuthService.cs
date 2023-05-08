using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using MockServer.Core.Identity;
using MockServer.Core.Repositories;

namespace MockServer.Web.Services;
public class AuthService : IAuthService
{
    public const string LoginProviderKey = "LoginProvider";
    private readonly IUserRepository _userRepository;

    
    public AuthService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public Task<User> AuthenticateExternalUser(ClaimsPrincipal externalUser)
    {
        throw new NotImplementedException();
    }

    public async Task<User> AuthenticateLocalUser(string email, string password)
    {
        var user = await _userRepository.FindUserByEmail(email);
        if (user == null)
        {
            return null;
        }
        if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        {
            return null;
        }
        return user;
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != passwordHash[i])
            {
                return false;
            }
        }

        return true;
    }

    public AuthenticationProperties ConfigureExternalAuthenticationProperties(string provider, string redirectUrl)
    {
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        properties.Items[LoginProviderKey] = provider;
        return properties;
    }

    public ClaimsPrincipal CreateUserPrincipal(IdentityUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            //new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
            // Add any other required claims here...
        };
        var identity = new ClaimsIdentity(claims, "LOCAL AUTHORITY");
        return new ClaimsPrincipal(identity);
    }
}