using Microsoft.AspNetCore.Mvc.ModelBinding;
using MycroCloud.WebMvc.Areas.Services.Models.WebApps;

namespace MycroCloud.WebMvc.Areas.Services.Services;

public interface IWebAppRouteService
{
    Task<RouteIndexViewModel> GetIndexViewModel(int appId, string searchTerm, string sort);
    Task<IEnumerable<RouteItem>> GetList(int appId, string searchTerm, string sort);
    Task<bool> ValidateCreate(int appId, RouteSaveModel route, ModelStateDictionary modelState);
    Task<int> Create(int appId, RouteSaveModel route);
    Task<bool> ValidateEdit(int routeId, RouteSaveModel route, ModelStateDictionary modelState);
    Task Edit(int routeId, RouteSaveModel route);
    Task Delete(int routeId);
    Task<RouteSaveModel> GetDetails(int routeId);
}
public class WebAppRouteService(IHttpContextAccessor contextAccessor) : BaseService(contextAccessor), IWebAppRouteService
{
    public async Task<RouteIndexViewModel> GetIndexViewModel(int appId, string searchTerm, string sort)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<RouteItem>> GetList(int appId, string searchTerm, string sort)
    {
        throw new NotImplementedException();
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
}
