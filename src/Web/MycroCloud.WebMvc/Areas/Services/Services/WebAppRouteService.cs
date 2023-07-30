using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using MycroCloud.WebMvc.Areas.Services.Models.WebApps;
using WebApp.Api.Grpc;

namespace MycroCloud.WebMvc.Areas.Services.Services;

public interface IWebAppRouteService
{
    Task<RouteIndexViewModel> GetIndexViewModel(string name, string searchTerm, string sort);
    Task<IEnumerable<RouteIndexItem>> GetList(string appName, string searchTerm, string sort);
    Task<RouteModel> GetRouteDetails(string appName, int routeId);
    Task<bool> ValidateCreate(int appId, RouteSaveModel route, ModelStateDictionary modelState);
    Task<int> Create(int appId, RouteSaveModel route);
    Task<bool> ValidateEdit(int routeId, RouteSaveModel route, ModelStateDictionary modelState);
    Task Edit(int routeId, RouteSaveModel route);
    Task Delete(int routeId);
    Task<RouteSaveModel> GetDetails(int routeId);
}
public class WebAppRouteService(IHttpContextAccessor contextAccessor
    , WebAppRoute.WebAppRouteClient webAppRouteClient
    , WebAppAuthentication.WebAppAuthenticationClient authenticationClient) : BaseService(contextAccessor), IWebAppRouteService
{
    private readonly WebAppRoute.WebAppRouteClient _webAppRouteClient = webAppRouteClient;
    private readonly WebAppAuthentication.WebAppAuthenticationClient _authenticationClient = authenticationClient;

    public async Task<RouteIndexViewModel> GetIndexViewModel(string name, string searchTerm, string sort)
    {
        var viewModel = new RouteIndexViewModel();
        var getRouteResponse = await _webAppRouteClient.GetAllAsync(new()
        {
            UserId = AuthUser.Id,
            AppName = name
        });
        viewModel.Routes = getRouteResponse.Routes.Select(r => new RouteIndexItem
        {
            RouteId = r.Id,
            Name = r.Name,
            Description = r.Description,
            MatchPath = r.MatchPath,
            MatchMethods = r.MatchMethods.Select(m => m).ToList(),
            MatchOrder = r.MatchOrder,
            AuthorizationType = (RouteAuthorizationType)r.AuthorizationType,
            ResponseProvider = (RouteResponseProvider)r.ResponseProvider,
            CreatedDate = r.CreatedDate.ToDateTime(),
            UpdatedDate = r.UpdatedDate?.ToDateTime()
        });
        viewModel.HttpMethodSelectListItem = CommonHttpMethods
                                                .Select(m => new SelectListItem(m.ToUpper(), m.ToUpper()))
                                                .Prepend(new SelectListItem("ALL", "ALL"));
        viewModel.AuthorizationAuthenticationSchemeSelectListItem = (await GetAuthenticationSchemeItems(name))
                                                        .Select(s => new SelectListItem(s.DisplayName, s.Id.ToString()));
        viewModel.AuthorizationPolicySelectListItem = new List<SelectListItem>();
        return viewModel;
    }
    private static List<string> CommonHttpMethods => new() {
        "GET", "HEAD", "POST", "PUT", "DELETE", "CONNECT", "OPTIONS", "TRACE", "PATCH"
    };

    private async Task<IEnumerable<AuthenticationSchemeIndexItem>> GetAuthenticationSchemeItems(string name)
    {
        var res = await _authenticationClient.GetAllAsync(new()
        {
            UserId = AuthUser.Id,
            AppName = name
        });
        return res.Schemes.Select(s => new AuthenticationSchemeIndexItem
        {
            Id = s.Id,
            Type = (AuthenticationSchemeType)(int)s.Type,
            Name = s.Name,
            DisplayName = s.DisplayName
        });
    }
    public async Task<IEnumerable<RouteIndexItem>> GetList(string appName, string searchTerm, string sort)
    {
        var res = await _webAppRouteClient.GetAllAsync(new()
        {
            UserId = AuthUser.Id,
            AppName = appName
        });
        return res.Routes.Select(r => new RouteIndexItem
        {
            RouteId = r.Id,
            Name = r.Name,
            MatchPath = r.MatchPath,
            MatchMethods = r.MatchMethods.Select(m => m).ToList(),
            MatchOrder = r.MatchOrder,
            CreatedDate = r.CreatedDate.ToDateTime(),
            UpdatedDate = r.UpdatedDate?.ToDateTime()
        });
    }

    public async Task<bool> ValidateCreate(int appId, RouteSaveModel route, ModelStateDictionary modelState)
    {
        throw new NotImplementedException();
    }

    public async Task<int> Create(int appId, RouteSaveModel route)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ValidateEdit(int routeId, RouteSaveModel route, ModelStateDictionary modelState)
    {
        throw new NotImplementedException();
    }

    public async Task Edit(int routeId, RouteSaveModel route)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(int routeId)
    {
        throw new NotImplementedException();
    }

    public async Task<RouteSaveModel> GetDetails(int routeId)
    {
        throw new NotImplementedException();
    }

    public async Task<RouteModel> GetRouteDetails(string appName, int routeId)
    {
        var res = await _webAppRouteClient.GetAsync(new()
        {
            UserId = AuthUser.Id,
            AppName = appName,
            Id = routeId
        });
        return new()
        {
            Id = res.Id,
            Name = res.Name,
            Description = res.Description,
            MatchPath = res.MatchPath,
            MatchMethods = res.MatchMethods.Select(m => m).ToList(),
            MatchOrder = res.MatchOrder,
            AuthorizationType = (RouteAuthorizationType)res.AuthorizationType,
            Authorization = !string.IsNullOrEmpty(res.AuthorizationJson) ? JsonSerializer.Deserialize<RouteAuthorization>(res.AuthorizationJson) : null,
            Validation = !string.IsNullOrEmpty(res.ValidationJson) ? JsonSerializer.Deserialize<RouteValidation>(res.ValidationJson) : null,
            ResponseProvider = (RouteResponseProvider)res.ResponseProvider,
            Response = !string.IsNullOrEmpty(res.ResponseJson) ? JsonSerializer.Deserialize<RouteResponse>(res.ResponseJson) : null,
            CreatedDate = res.CreatedDate.ToDateTime(),
            UpdatedDate = res.UpdatedDate?.ToDateTime()
        };
    }
}
