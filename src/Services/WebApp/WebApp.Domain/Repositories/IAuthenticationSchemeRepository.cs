using WebApp.Domain.Entities;

namespace WebApp.Domain.Repositories;

public interface IAuthenticationSchemeRepository
{
    Task AddJwtBearerScheme(int appId, JwtBearerAuthenticationSchemeEntity scheme);
    Task UpdateJwtBearerScheme(int appId, JwtBearerAuthenticationSchemeEntity scheme);
    Task<IEnumerable<AuthenticationSchemeEntity>> GetAllByAppId(int appId);
    Task<JwtBearerAuthenticationSchemeEntity> GetJwtBearerScheme(int schemeId);
}