using MicroCloud.Web.Models.WebApplications.Authorizations;

namespace MicroCloud.Web.Services;

public interface IWebApplicationAuthorizationWebService
{
    Task<PolicyIndexViewModel> GetPolicyIndexViewModel(int appId);
    Task CreatePolicy(int appId, PolicySaveModel policy);
    Task EditPolicy(int policyId, PolicySaveModel policy);
    Task<PolicySaveModel> GetPolicyCreateModel(int appId);
    Task<PolicySaveModel> GetPolicyEditModel(int policyId);
    Task Delete(int policyId);
}
