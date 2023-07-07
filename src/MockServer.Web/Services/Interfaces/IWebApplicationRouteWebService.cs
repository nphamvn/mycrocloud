using Microsoft.AspNetCore.Mvc.ModelBinding;
using MockServer.Web.Models.WebApplications.Routes;

namespace MockServer.Web.Services;

public interface IWebApplicationRouteService
{
    Task<RouteIndexViewModel> GetIndexViewModel(int appId, string searchTerm, string sort);
    Task<IEnumerable<RouteIndexItem>> GetList(int appId, string searchTerm, string sort);
    Task<bool> ValidateCreate(int appId, RouteSaveModel route, ModelStateDictionary modelState);
    Task<int> Create(int appId, RouteSaveModel route);
    Task<bool> ValidateEdit(int routeId, RouteSaveModel route, ModelStateDictionary modelState);
    Task Edit(int routeId, RouteSaveModel route);
    Task Delete(int routeId);
    Task<RouteSaveModel> GetDetails(int routeId);
}
