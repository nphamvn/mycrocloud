using System.Security.Claims;
using MockServer.Core.Models.Authorization;

namespace MockServer.Core.Interfaces;

public interface IJwtBearerAuthorization {
    string GenerateToken(JwtHandlerConfiguration options);
    ClaimsPrincipal Validate(string token, JwtHandlerConfiguration options);
}