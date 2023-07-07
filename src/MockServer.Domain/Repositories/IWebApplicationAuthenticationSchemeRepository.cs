using MockServer.Domain.WebApplication.Entities;
using MockServer.Domain.WebApplication.Shared;

namespace MockServer.Domain.Repositories;

public interface IWebApplicationAuthenticationSchemeRepository
{
    Task Add(int appId, AuthenticationSchemeEntity schemeEntity);
    Task Update(int appId, AuthenticationSchemeEntity schemeEntity);
    Task Activate(int appId, List<int> schemeIds);
    Task<IEnumerable<AuthenticationSchemeEntity>> GetAll(int appId);
    Task<AuthenticationSchemeEntity> Get(int schemeId);
    Task<AuthenticationSchemeEntity> Get(int schemeId, AuthenticationSchemeType type);
    Task<AuthenticationSchemeEntity> Get<TOptions>(int schemeId) where TOptions : AuthenticationSchemeOptions;
}