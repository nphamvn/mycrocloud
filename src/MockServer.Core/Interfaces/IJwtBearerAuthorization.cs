using MockServer.Core.Models.Auth;

namespace MockServer.Core.Interfaces;

public interface IJwtBearerTokenService
{
    string GenerateToken(JwtBearerAuthenticationOptions options);
}