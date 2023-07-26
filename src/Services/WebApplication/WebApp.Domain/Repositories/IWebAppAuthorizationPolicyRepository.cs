using WebApp.Domain.Entities;

namespace WebApp.Domain.Repositories;

public interface IWebAppAuthorizationPolicyRepository
{
    Task Add(int appId, AuthorizationPolicyEntity authorizationPolicyEntity);
    Task Update(int appId, AuthorizationPolicyEntity authorizationPolicyEntity);
    Task<IEnumerable<AuthorizationPolicyEntity>> GetAll(int appId);
    Task<AuthorizationPolicyEntity> Get(int policyId);
    Task Delete(int policyId);
}