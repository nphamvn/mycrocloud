using WebApp.Domain.Entities;

namespace WebApp.Domain.Repositories;

public interface IAuthenticationSchemeRepository
{
    Task AddJwtBearerScheme(int appId, JwtBearerAuthenticationScheme scheme);
    Task UpdateJwtBearerScheme(int appId, JwtBearerAuthenticationScheme scheme);
    Task<IEnumerable<AuthenticationScheme>> GetAllByAppId(int appId);
    Task<JwtBearerAuthenticationScheme> GetJwtBearerScheme(int schemeId);
}