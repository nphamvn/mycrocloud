using MockServer.Domain.WebApplication.Entities;

namespace MockServer.Domain.Repositories;

public interface IWebApplicationAuthorizationPolicyRepository
{
    Task Add(int appId, PolicyEntity policyEntity);
    Task Update(int appId, PolicyEntity policyEntity);
    Task<IEnumerable<PolicyEntity>> GetAll(int appId);
    Task<PolicyEntity> Get(int policyId);
    Task Delete(int policyId);
}