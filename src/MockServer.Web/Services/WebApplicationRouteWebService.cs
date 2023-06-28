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
    private readonly IWebApplicationAuthenticationSchemeRepository _webApplicationAuthenticationSchemeRepository;
    private readonly IWebApplicationAuthorizationPolicyRepository _webApplicationAuthorizationPolicyRepository;
    private readonly IWebApplicationService _webApplicationWebService;
    private readonly IMapper _mapper;
    public WebApplicationRouteService(
        IHttpContextAccessor contextAccessor,
        IWebApplicationRouteRepository requestRepository,
        //IWebApplicationRepository projectRepository,
        IWebApplicationAuthenticationSchemeRepository webApplicationAuthenticationSchemeRepository,
        IWebApplicationAuthorizationPolicyRepository webApplicationAuthorizationPolicyRepository,
        IWebApplicationService webApplicationWebService,
        IMapper mapper) : base(contextAccessor)
    {
        _webApplicationRouteRepository = requestRepository;
        //_webApplicationRepository = projectRepository;
        _webApplicationAuthenticationSchemeRepository = webApplicationAuthenticationSchemeRepository;
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
                //Path = route.Path,
                //Method = route.Methods[0],
                Authorization = new ()
                {
                    Type = route.Authorization.Type,
                    PolicyIds = route.Authorization.PolicyIds,
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
            HttpMethodSelectListItem = HttpProtocolExtensions.CommonHttpMethods
                                    .Select(m => new SelectListItem(m, m)),
            ResponseProviderSelectListItem = new List<SelectListItem>{
                new("Mock", ((int)ResponseProvider.Mock).ToString()),
                new("Proxied Server", ((int)ResponseProvider.ProxiedServer).ToString()),
                new("Function", ((int)ResponseProvider.Function).ToString())
            },
            AuthorizationTypeSelectListItem = new List<SelectListItem>
            {
                new("Allow Anonymous", ((int)AuthorizationType.AllowAnonymous).ToString()),
                new("Authorize", ((int)AuthorizationType.Authorized).ToString())
            },
        };
        vm.AuthorizationAuthenticationSchemeSelectListItem = 
            (await _webApplicationAuthenticationSchemeRepository.GetAll(appId)).Select(s => new SelectListItem(s.Name, s.SchemeId.ToString()));
        var policies = await _webApplicationAuthorizationPolicyRepository.GetAll(appId);
        vm.AuthorizationPolicySelectListItem = policies.Select(p => new SelectListItem(p.Name, p.PolicyId.ToString()));

        var routes = await _webApplicationRouteRepository.GetByApplicationId(appId, searchTerm, sort);
        vm.Routes = routes.Select(r => new RouteIndexItem()
        {
            RouteId = r.RouteId,
            Name = r.Name,
            Description = r.Description,
            //Methods = r.Methods,
            //Path = r.Path,
            //ResponseProvider = r.ResponseProvider
        });
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
            //Order = route.Order,
            //Path = route.Path,
            //Methods = route.Methods,
            
            Authorization = new AuthorizationViewModel()
            {
                Type = route.Authorization.Type,
                PolicyIds = route.Authorization.PolicyIds,
                Claims = route.Authorization.Claims
            },
            
        };
    }

    private RouteViewModel Map(CoreRoute route) {
        var vm = new RouteViewModel();
        vm.RouteId = route.RouteId;
        vm.Name = route.Name;
        vm.Description = route.Description;
        if (route.Match != null)
        {
            vm.Match = new()
            {
                Order = route.Match.Order,
                Methods = route.Match.Methods,
                Path = route.Match.Path
            };
        }
        if (route.Authorization != null)
        {
            vm.Authorization = new()
            {
                Type = route.Authorization.Type,
                AuthenticationSchemeIds = 
            };
        }

        return vm;
    }
}
