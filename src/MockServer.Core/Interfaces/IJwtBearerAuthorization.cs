using System.Security.Claims;
using MockServer.Core.WebApplications.Security.JwtBearer;

namespace MockServer.Core.Interfaces;

public interface IJwtBearerTokenService
{
    string GenerateToken(JwtBearerAuthenticationOptions options, List<Claim> claims);
    ClaimsPrincipal ValidateToken(string token, JwtBearerAuthenticationOptions options);
}