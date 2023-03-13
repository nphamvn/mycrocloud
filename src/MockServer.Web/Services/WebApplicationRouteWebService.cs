using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Core.Helpers;
using MockServer.Core.Repositories;
using MockServer.Core.WebApplications;
using MockServer.Web.Models.WebApplications.Routes;
using MockServer.Web.Models.WebApplications.Routes.Authorizations;
using MockServer.Web.Models.WebApplications.Routes.Integrations.MockIntegrations;
using CoreRoute = MockServer.Core.WebApplications.Route;
using CoreAuthorization = MockServer.Core.WebApplications.Security.Authorization;
using CoreMockIntegration = MockServer.Core.WebApplications.MockIntegration;
using MockServer.Core.WebApplications.Security;

namespace MockServer.Web.Services;

public class WebApplicationRouteWebService : BaseWebService, IWebApplicationRouteWebService
{
    private readonly IWebApplicationRouteRepository _webApplicationRouteRepository;
    private readonly IWebApplicationRepository _webApplicationRepository;
    private readonly IWebApplicationAuthenticationSchemeRepository _authRepository;
    private readonly IWebApplicationAuthorizationPolicyRepository _webApplicationAuthorizationPolicyRepository;
    private readonly IWebApplicationWebService _webApplicationWebService;
    private readonly IMapper _mapper;
    public WebApplicationRouteWebService(
        IHttpContextAccessor contextAccessor,
        IWebApplicationRouteRepository requestRepository,
        IWebApplicationRepository projectRepository,
        IWebApplicationAuthenticationSchemeRepository authRepository,
        IWebApplicationAuthorizationPolicyRepository webApplicationAuthorizationPolicyRepository,
        IWebApplicationWebService webApplicationWebService,
        IMapper mapper) : base(contextAccessor)
    {
        _webApplicationRouteRepository = requestRepository;
        _webApplicationRepository = projectRepository;
        _authRepository = authRepository;
        _webApplicationAuthorizationPolicyRepository = webApplicationAuthorizationPolicyRepository;
        _webApplicationWebService = webApplicationWebService;
        _mapper = mapper;
    }
    public async Task<RouteViewModel> GetViewModel(int routeId)
    {
        var route = await _webApplicationRouteRepository.GetById(routeId);
        var vm = _mapper.Map<RouteViewModel>(route);
        vm.WebApplication = await _webApplicationWebService.Get(route.WebApplicationId);
        vm.WebApplication.User = AuthUser;
        if (route.IntegrationType == RouteIntegrationType.MockIntegration)
        {
            var integration = _mapper.Map<MockIntegrationViewModel>(await _webApplicationRouteRepository.GetMockIntegration(routeId));
            integration.ResponseStatusCodeSelectListItems = HttpProtocolExtensions.CommonHttpStatusCode
                                                            .Select(c => new SelectListItem(c.ToString(), ((int)c).ToString()));
            vm.Integration = integration;
        }
        else if (route.IntegrationType == RouteIntegrationType.DirectForwarding)
        {

        }
        else if (route.IntegrationType == RouteIntegrationType.FunctionTrigger)
        {

        }
        vm.MethodSelectListItem = HttpProtocolExtensions.CommonHttpMethods
                                    .Select(m => new SelectListItem(m, m));
        vm.Authorization.AuthorizationTypeSelectListItem = new List<SelectListItem>
        {
            new("None", nameof(AuthorizationType.None)),
            new("Allow Anonymous", nameof(AuthorizationType.AllowAnonymous)),
            new("Authorize", nameof(AuthorizationType.Authorize))
        };
        var policies = await _webApplicationAuthorizationPolicyRepository.GetAll(vm.WebApplication.Id);
        vm.Authorization.PolicySelectListItem = policies
                        .Select(p => new SelectListItem {
                            Value = p.Id.ToString(),
                            Text = p.Name,
                            Selected = vm.Authorization.PolicyIds.Contains(p.Id)
                        });
        return vm;
    }

    public async Task<int> Create(int appId, RouteSaveModel route)
    {
        var existing = await _webApplicationRouteRepository.Find(appId, route.Method, route.Path);
        if (existing == null)
        {
            var mapped = _mapper.Map<CoreRoute>(route);
            return await _webApplicationRouteRepository.Create(appId, mapped);
        }
        else
        {
            return 0;
        }
    }

    public async Task Delete(int id)
    {
        await _webApplicationRouteRepository.Delete(id);
    }

    public async Task<RouteSaveModel> GetEditRouteModel(int requestId)
    {
        var route = await _webApplicationRouteRepository.GetById(requestId);
        var vm = _mapper.Map<RouteSaveModel>(route);
        vm.WebApplication = await _webApplicationWebService.Get(route.WebApplicationId);
        vm.HttpMethodSelectListItems = HttpProtocolExtensions.CommonHttpMethods
                                        .Select(m => new SelectListItem(m, m));
        return vm;
    }

    public async Task<bool> ValidateEdit(int id, RouteSaveModel request, ModelStateDictionary modelState)
    {
        return modelState.IsValid;
    }

    public async Task Edit(int id, RouteSaveModel route)
    {
        var existing = await _webApplicationRouteRepository.GetById(id);
        if (existing != null)
        {
            var mapped = _mapper.Map<CoreRoute>(route);
            await _webApplicationRouteRepository.Update(id, mapped);
        }
    }

    public async Task<AuthorizationViewModel> GetAuthorizationViewModel(int appId, int routeId)
    {
        var authorization = await _webApplicationRouteRepository.GetAuthorization(routeId);
        var vm = authorization != null ? _mapper.Map<AuthorizationViewModel>(authorization)
                                        : new AuthorizationViewModel();
        //var AuthenticationSchemes = await _webApplicationWebService.Get()
        //vm.AuthenticationSchemeSelectListItems = await _webApplicationWebService.(projectId);
        return vm;
    }

    public async Task AttachAuthorization(int routeId, AuthorizationSaveModel auth)
    {
        var authorization = _mapper.Map<CoreAuthorization>(auth);
        await _webApplicationRouteRepository.AttachAuthorization(routeId, authorization);
    }

    public async Task<RouteIndexModel> GetIndexModel(int appId)
    {
        var vm = new RouteIndexModel
        {
            WebApplication = await _webApplicationWebService.Get(appId),
            Routes = _mapper.Map<IEnumerable<RouteIndexItem>>(await _webApplicationRouteRepository.GetByApplicationId(appId))
        };
        return vm;
    }

    public Task<bool> ValidateCreate(int appId, RouteSaveModel request, ModelStateDictionary modelState)
    {
        return Task.FromResult(modelState.IsValid);
    }

    public async Task<RouteSaveModel> GetCreateRouteModel(int appId)
    {
        var vm = new RouteSaveModel();
        vm.WebApplication = await _webApplicationWebService.Get(appId);
        vm.WebApplication.User = AuthUser;
        vm.HttpMethodSelectListItems = HttpProtocolExtensions.CommonHttpMethods
                                        .Select(m => new SelectListItem(m, m));
        return vm;
    }

    public async Task<MockIntegrationViewModel> GetMockIntegration(int routeId)
    {
        var integration = await _webApplicationRouteRepository.GetMockIntegration(routeId);
        return _mapper.Map<MockIntegrationViewModel>(integration);
    }

    public async Task SaveMockIntegration(int requestId, MockIntegrationSaveModel integration)
    {
        var mapped = _mapper.Map<CoreMockIntegration>(integration);
        await _webApplicationRouteRepository.UpdateMockIntegration(requestId, mapped);
    }

    public Task ChangeIntegrationType(int routeId, RouteIntegrationType type)
    {
        throw new NotImplementedException();
    }
}