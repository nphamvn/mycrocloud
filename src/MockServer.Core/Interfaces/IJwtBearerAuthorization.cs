using System.Security.Claims;
using MockServer.Core.Models.Auth;

namespace MockServer.Core.Interfaces;

public interface IJwtBearerTokenService
{
    string GenerateToken(JwtBearerAuthenticationOptions options);
    ClaimsPrincipal ValidateToken(string token, JwtBearerAuthenticationOptions options);
}