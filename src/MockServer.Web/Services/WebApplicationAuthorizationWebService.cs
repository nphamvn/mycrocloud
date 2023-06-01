using AutoMapper;
using MockServer.Core.Repositories;
using MockServer.Web.Models.WebApplications.Authorizations;
using CorePolicy = MockServer.Core.WebApplications.Security.Policy;
namespace MockServer.Web.Services;

public class WebApplicationAuthorizationWebService : BaseWebService, IWebApplicationAuthorizationWebService
{
    private readonly IWebApplicationAuthorizationPolicyRepository _webApplicationAuthorizationPolicyRepository;
    private readonly IWebApplicationWebService _webApplicationWebService;
    private readonly IMapper _mapper;

    public WebApplicationAuthorizationWebService(
        IWebApplicationAuthorizationPolicyRepository webApplicationAuthorizationPolicyRepository,
        IWebApplicationWebService webApplicationWebService,
        IMapper mapper,
        IHttpContextAccessor contextAccessor) : base(contextAccessor)
    {
        _webApplicationAuthorizationPolicyRepository = webApplicationAuthorizationPolicyRepository;
        _webApplicationWebService = webApplicationWebService;
        _mapper = mapper;
    }

    public async Task CreatePolicy(int appId, PolicySaveModel model)
    {
        var policy = new CorePolicy {
            Name = model.Name,
            Description = model.Description,
            Claims = (Core.WebApplications.Security.Claims)model.Claims.ToDictionary(k => k.Key, v => v.Value)
        };
        await _webApplicationAuthorizationPolicyRepository.Add(appId, policy);
    }

    public Task Delete(int policyId)
    {
        return _webApplicationAuthorizationPolicyRepository.Delete(policyId);
    }

    public async Task EditPolicy(int policyId, PolicySaveModel model)
    {
        var claims = new Core.WebApplications.Security.Claims();
        model.Claims.ForEach(c => claims.Add(c.Key, c.Value));
        var policy = new CorePolicy {
            Name = model.Name,
            Description = model.Description,
            Claims = claims
        };
        await _webApplicationAuthorizationPolicyRepository.Update(policyId, policy);
    }

    public async Task<PolicySaveModel> GetPolicyCreateModel(int appId)
    {
        var model = new PolicySaveModel();
        model.WebApplication = await _webApplicationWebService.Get(appId);
        return model;
    }

    public async Task<PolicySaveModel> GetPolicyEditModel(int policyId)
    {
        var policy = await _webApplicationAuthorizationPolicyRepository.Get(policyId);
        var model =  _mapper.Map<PolicySaveModel>(policy);
        model.WebApplication = await _webApplicationWebService.Get(policy.WebApplicationId);
        return model;
    }

    public async Task<PolicyIndexViewModel> GetPolicyIndexViewModel(int appId)
    {
        var vm = new PolicyIndexViewModel();
        vm.WebApplication = await _webApplicationWebService.Get(appId);
        var policies = await _webApplicationAuthorizationPolicyRepository.GetAll(appId);
        vm.Policies = _mapper.Map<IEnumerable<PolicyIndexItem>>(policies);
        return vm;
    }
}
