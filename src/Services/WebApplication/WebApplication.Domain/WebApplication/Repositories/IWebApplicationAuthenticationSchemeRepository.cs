using WebApplication.Domain.WebApplication.Entities;
using WebApplication.Domain.WebApplication.Shared;

namespace WebApplication.Domain.Repositories;

public interface IWebApplicationAuthenticationSchemeRepository
{
    Task Add(int appId, AuthenticationScheme scheme);
    Task Update(int appId, AuthenticationScheme scheme);
    Task Activate(int appId, List<int> schemeIds);
    Task<IEnumerable<AuthenticationScheme>> GetAll(int appId);
    Task<AuthenticationScheme> Get(int schemeId);
    Task<AuthenticationScheme> Get(int schemeId, AuthenticationSchemeType type);
    Task<AuthenticationScheme> Get<TOptions>(int schemeId) where TOptions : AuthenticationSchemeOptions;
}