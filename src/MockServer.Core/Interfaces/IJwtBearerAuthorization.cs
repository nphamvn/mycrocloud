using MockServer.Core.Models.Auth;
using MockServer.Core.Services.Auth;

namespace MockServer.Core.Interfaces;

public interface IJwtBearerTokenService
{
    string GenerateToken(JwtBearerAuthenticationOptions options);
    JwtBearerAuthHandler BuildHandler(string token, JwtBearerAuthenticationOptions options);
}