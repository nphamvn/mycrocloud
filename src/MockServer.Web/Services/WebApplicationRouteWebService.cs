using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Core.Helpers;
using MockServer.Core.Repositories;
using MockServer.Core.WebApplications;
using MockServer.Web.Models.WebApplications.Routes;
using CoreRoute = MockServer.Core.WebApplications.Route;
using MockServer.Core.WebApplications.Security;
using MockServer.Web.Models.WebApplications.Routes.Authorizations;
using MockServer.Web.Shared;

namespace MockServer.Web.Services;

public class WebApplicationRouteService : BaseService, IWebApplicationRouteService
{
    private readonly IWebApplicationRouteRepository _webApplicationRouteRepository;
    //private readonly IWebApplicationRepository _webApplicationRepository;
    //private readonly IWebApplicationAuthenticationSchemeRepository _authRepository;
    private readonly IWebApplicationAuthorizationPolicyRepository _webApplicationAuthorizationPolicyRepository;
    private readonly IWebApplicationService _webApplicationWebService;
    private readonly IMapper _mapper;
    public WebApplicationRouteService(
        IHttpContextAccessor contextAccessor,
        IWebApplicationRouteRepository requestRepository,
        //IWebApplicationRepository projectRepository,
        //IWebApplicationAuthenticationSchemeRepository authRepository,
        IWebApplicationAuthorizationPolicyRepository webApplicationAuthorizationPolicyRepository,
        IWebApplicationService webApplicationWebService,
        IMapper mapper) : base(contextAccessor)
    {
        _webApplicationRouteRepository = requestRepository;
        //_webApplicationRepository = projectRepository;
        //_authRepository = authRepository;
        _webApplicationAuthorizationPolicyRepository = webApplicationAuthorizationPolicyRepository;
        _webApplicationWebService = webApplicationWebService;
        _mapper = mapper;
    }

    public async Task<int> Create(int appId, RouteSaveModel route)
    {
        var existing = await _webApplicationRouteRepository.Find(appId, route.Methods[0], route.Path);
        if (existing == null)
        {
            return await _webApplicationRouteRepository.Create(appId, new()
            {
                Name = route.Name,
                Description = route.Description,
                Path = route.Path,
                Method = route.Methods[0],
                Authorization = new ()
                {
                    Type = route.Authorization.Type,
                    PolicyIds = route.Authorization.Policies,
                }
            });
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

    public Task<bool> ValidateCreate(int appId, RouteSaveModel request, ModelStateDictionary modelState)
    {
        return Task.FromResult(modelState.IsValid);
    }

    public async Task<RouteIndexViewModel> GetIndexViewModel(int appId, string searchTerm, string sort)
    {
        var vm = new RouteIndexViewModel
        {
            WebApplication = await _webApplicationWebService.Get(appId),
            Routes = _mapper.Map<IEnumerable<RouteIndexItem>>(await _webApplicationRouteRepository.GetByApplicationId(appId, searchTerm, sort)),
            HttpMethodSelectListItem = HttpProtocolExtensions.CommonHttpMethods
                                    .Select(m => new SelectListItem(m, m)),
            ResponseProviderSelectListItem = new List<SelectListItem>{
                new("Mock Response", ((int)ResponseProvider.MockResponse).ToString()),
                new("Forward Proxy", ((int)ResponseProvider.RequestForward).ToString()),
                new("Function Trigger", ((int)ResponseProvider.Function).ToString())
            },
            AuthorizationTypeSelectListItem = new List<SelectListItem>
            {
                //new("None", ((int)AuthorizationType.None).ToString()),
                new("Allow Anonymous", ((int)AuthorizationType.AllowAnonymous).ToString()),
                new("Authorize", ((int)AuthorizationType.Authorized).ToString())
            },
        };
        var policies = await _webApplicationAuthorizationPolicyRepository.GetAll(appId);
        vm.AuthorizationPolicySelectListItem = policies.Select(p => new SelectListItem(p.Name, p.PolicyId.ToString()));
        vm.BuiltInValdationAttributes = new List<BuiltInValdationAttributeDescription>
        {
            new () {
                Name = "Required"
            },
            new () {
                Name = "Range",
                ParameterDescription = "Type, Min, Max"
            }
        };
        return vm;
    }

    public async Task<IEnumerable<RouteIndexItem>> GetList(int appId, string searchTerm, string sort)
    {
        var routes = await _webApplicationRouteRepository.GetByApplicationId(appId, searchTerm, sort);
        return _mapper.Map<IEnumerable<RouteIndexItem>>(routes);
    }

    public async Task<RouteViewModel> GetDetails(int routeId)
    {
        var route = await _webApplicationRouteRepository.GetById(routeId);
        return new RouteViewModel()
        {
            RouteId = routeId,
            Name = route.Name,
            Description = route.Description,
            Order = route.Order,
            Path = route.Path,
            Methods = route.Methods,
            Authorization = new AuthorizationViewModel()
            {
                Type = route.Authorization.Type,
                PolicyIds = route.Authorization.PolicyIds,
                Claims = route.Authorization.Claims
            },
            
        };
    }
}
