using Microsoft.AspNetCore.Mvc.ModelBinding;
using MockServer.Core.WebApplications;
using MockServer.Web.Models.WebApplications.Routes;
using MockServer.Web.Models.WebApplications.Routes.Authorizations;
using MockServer.Web.Models.WebApplications.Routes.Integrations.MockIntegrations;

namespace MockServer.Web.Services;

public interface IWebApplicationRouteWebService
{
    Task<RouteIndexModel> GetIndexModel(int appId, string searchTerm, string sort);
    Task<RoutePageModel> GetPageModel(int appId, string searchTerm, string sort);
    Task<bool> ValidateCreate(int appId, RouteSaveModel route, ModelStateDictionary modelState);
    Task<int> Create(int appId, RouteSaveModel route);
    Task<bool> ValidateEdit(int routeId, RouteSaveModel route, ModelStateDictionary modelState);
    Task Edit(int routeId, RouteSaveModel route);
    Task<RouteSaveModel> GetEditRouteModel(int routeId);
    Task<RouteSaveModel> GetCreateRouteModel(int appId);
    Task Delete(int routeId);
    Task<RouteViewModel> GetViewModel(int routeId);
    Task<AuthorizationViewModel> GetAuthorizationViewModel(int appId, int requestId);
    Task AttachAuthorization(int requestId, AuthorizationSaveModel auth);
    Task<MockIntegrationViewModel> GetMockIntegration(int requestId);
    Task SaveMockIntegration(int requestId, MockIntegrationSaveModel integration);
    Task ChangeIntegrationType(int routeId, RouteIntegrationType type);
}
