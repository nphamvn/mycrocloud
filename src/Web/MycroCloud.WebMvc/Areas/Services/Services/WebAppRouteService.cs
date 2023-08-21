using System.Text.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using MycroCloud.WebApp;
using MycroCloud.WebMvc.Areas.Services.Models.WebApps;
using static MycroCloud.WebApp.WebAppAuthenticationGrpcService;
using static MycroCloud.WebApp.WebAppRouteGrpcService;

namespace MycroCloud.WebMvc.Areas.Services.Services;

public interface IWebAppRouteService
{
    Task<RouteIndexViewModel> GetIndexViewModel(int appId, string searchTerm, string sort);
    Task<IEnumerable<RouteIndexItem>> GetList(int appId, string searchTerm, string sort);
    Task<RouteModel> GetRouteDetails(int appId, int routeId);
    Task<bool> ValidateCreate(int appId, RouteSaveModel route, ModelStateDictionary modelState);
    Task<int> Create(int appId, RouteSaveModel route);
    Task<bool> ValidateEdit(int routeId, RouteSaveModel route, ModelStateDictionary modelState);
    Task Edit(int routeId, RouteSaveModel route);
    Task Delete(int routeId);
    Task<RouteSaveModel> GetDetails(int routeId);
}
public class WebAppRouteService(WebAppRouteGrpcServiceClient webAppRouteGrpcServiceClient
    , WebAppAuthenticationGrpcServiceClient webAppAuthenticationGrpcServiceClient) : IWebAppRouteService
{
    public async Task<RouteIndexViewModel> GetIndexViewModel(int appId, string searchTerm, string sort)
    {
        var viewModel = new RouteIndexViewModel();
        var getRouteResponse = await webAppRouteGrpcServiceClient.ListRoutesAsync(new()
        {

        });
        viewModel.Routes = getRouteResponse.Routes.Select(r => new RouteIndexItem
        {
            RouteId = r.RouteId,
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
        viewModel.AuthorizationAuthenticationSchemeSelectListItem = (await GetAuthenticationSchemeItems(appId))
                                                        .Select(s => new SelectListItem(s.DisplayName, s.Id.ToString()));
        viewModel.AuthorizationPolicySelectListItem = new List<SelectListItem>();
        return viewModel;
    }
    private static List<string> CommonHttpMethods => new() {
        "GET", "HEAD", "POST", "PUT", "DELETE", "CONNECT", "OPTIONS", "TRACE", "PATCH"
    };

    private async Task<IEnumerable<AuthenticationSchemeIndexItem>> GetAuthenticationSchemeItems(int appId)
    {
        var res = await webAppAuthenticationGrpcServiceClient.GetAllAsync(new()
        {

        });
        return res.Schemes.Select(s => new AuthenticationSchemeIndexItem
        {
            Id = s.SchemeId,
            Type = (AuthenticationSchemeType)(int)s.Type,
            Name = s.Name,
            DisplayName = s.DisplayName
        });
    }
    public async Task<IEnumerable<RouteIndexItem>> GetList(int appId, string searchTerm, string sort)
    {
        var res = await webAppRouteGrpcServiceClient.ListRoutesAsync(new()
        {

        });
        return res.Routes.Select(r => new RouteIndexItem
        {
            RouteId = r.RouteId,
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
        var req = new CreateRouteRequest
        {
            AppId = appId,
            Name = route.Name,
            Description = route.Description,
            MatchPath = route.MatchPath,
            MatchOrder = route.MatchOrder ?? 1,
            ResponseType = (int)route.ResponseProvider,
        };
        req.MatchMethods.AddRange(route.MatchMethods);
        switch (route.ResponseProvider)
        {
            case RouteResponseProvider.Mock:
                var mockResponse = (MockResponseSaveModel)route.Response;
                req.MockResponse = new MockResponse();
                switch (mockResponse.StatusCode.ValueType)
                {
                    case MockResponseValueGenerator.Static:
                        req.MockResponse.StatusCode = new MockResponse.Types.StatusCode
                        {
                            Code = mockResponse.StatusCode.Code.Value
                        };
                        break;
                    case MockResponseValueGenerator.ExpressionEvaluated:
                        req.MockResponse.StatusCode = new MockResponse.Types.StatusCode
                        {
                            EvaluatedExpresion = mockResponse.StatusCode.Expression,
                        };
                        break;
                    default:
                        break;
                }
                foreach (var header in mockResponse.Headers)
                {
                    switch (header.Value.ValueType)
                    {
                        case MockResponseValueGenerator.Static:
                            req.MockResponse.Headers.Add(header.Key, new MockResponse.Types.Header
                            {
                                StaticValue = header.Value.StaticValue
                            });
                            break;
                        case MockResponseValueGenerator.ExpressionEvaluated:
                            req.MockResponse.Headers.Add(header.Key, new MockResponse.Types.Header
                            {
                                EvaluatedExpresion = header.Value.EvaluatedExpression
                            });
                            break;
                        default:
                            break;
                    }
                    
                }
                break;
            case RouteResponseProvider.ProxiedServer:
                req.ProxiedServer = new ProxiedServer
                {

                };
                break;
            default:
                break;
        }
        var res = await webAppRouteGrpcServiceClient.CreateRouteAsync(req);
        return res.RouteId;
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

    public async Task<RouteModel> GetRouteDetails(int appId, int routeId)
    {
        var res = await webAppRouteGrpcServiceClient.GetRouteByIdAsync(new()
        {
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
