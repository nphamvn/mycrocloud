using WebApp.Domain.Entities;

namespace WebApp.Domain.Repositories;

public interface IAuthorizationPolicyRepository
{
    Task Add(int policyId, AuthorizationPolicyEntity policy);
    Task Update(int policyId, AuthorizationPolicyEntity policy);
    Task<IEnumerable<AuthorizationPolicyEntity>> GetAllByAppId(int appId);
    Task<AuthorizationPolicyEntity> Get(int policyId);
    Task Delete(int policyId);
}