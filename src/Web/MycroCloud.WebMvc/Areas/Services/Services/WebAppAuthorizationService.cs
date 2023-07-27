using MycroCloud.WebMvc.Areas.Services.Models.WebApps;

namespace MycroCloud.WebMvc.Areas.Services.Services;
public interface IWebAppAuthorizationService
{
    Task<AuthorizationPolicyListViewModel> GetPolicyListViewModel(int appId);
    Task CreatePolicy(int appId, AuthorizationPolicySaveViewModel policy);
    Task EditPolicy(int policyId, AuthorizationPolicySaveViewModel policy);
    Task<AuthorizationPolicySaveViewModel> GetPolicyCreateModel(int appId);
    Task<AuthorizationPolicySaveViewModel> GetPolicyEditModel(int policyId);
    Task Delete(int policyId);
}

public class WebAppAuthorizationService : BaseService, IWebAppAuthorizationService
{
    public WebAppAuthorizationService(IHttpContextAccessor contextAccessor) : base(contextAccessor)
    {
    }

    public async Task<AuthorizationPolicyListViewModel> GetPolicyListViewModel(int appId)
    {
        throw new NotImplementedException();
    }

    public async Task CreatePolicy(int appId, AuthorizationPolicySaveViewModel policy)
    {
        throw new NotImplementedException();
    }

    public async Task EditPolicy(int policyId, AuthorizationPolicySaveViewModel policy)
    {
        throw new NotImplementedException();
    }

    public async Task<AuthorizationPolicySaveViewModel> GetPolicyCreateModel(int appId)
    {
        throw new NotImplementedException();
    }

    public async Task<AuthorizationPolicySaveViewModel> GetPolicyEditModel(int policyId)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(int policyId)
    {
        throw new NotImplementedException();
    }
}
