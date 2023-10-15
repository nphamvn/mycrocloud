using WebApp.Domain.Entities;

namespace WebApp.Domain.Repositories;

public interface IAuthorizationPolicyRepository
{
    Task Add(int policyId, AuthorizationPolicy policy);
    Task Update(int policyId, AuthorizationPolicy policy);
    Task<IEnumerable<AuthorizationPolicy>> GetAllByAppId(int appId);
    Task<AuthorizationPolicy> Get(int policyId);
    Task Delete(int policyId);
}