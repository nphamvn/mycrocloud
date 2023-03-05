using MockServer.Core.WebApplications.Security;

namespace MockServer.Core.Repositories;

public interface IWebApplicationAuthorizationPolicyRepository
{
    Task Add(int appId, Policy policy);
    Task Update(int appId, Policy policy);
    Task<IEnumerable<Policy>> GetAll(int appId);
    Task<Policy> Get(int policyId);
    Task Delete(int policyId);
}