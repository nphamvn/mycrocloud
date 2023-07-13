using WebApplication.Domain.WebApplication.Entities;

namespace WebApplication.Domain.Repositories;

public interface IWebApplicationAuthorizationPolicyRepository
{
    Task Add(int appId, PolicyEntity policyEntity);
    Task Update(int appId, PolicyEntity policyEntity);
    Task<IEnumerable<PolicyEntity>> GetAll(int appId);
    Task<PolicyEntity> Get(int policyId);
    Task Delete(int policyId);
}