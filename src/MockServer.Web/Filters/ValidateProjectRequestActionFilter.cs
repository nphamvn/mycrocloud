using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MockServer.Domain.Repositories;

namespace MockServer.Web.Filters;

public class ValidateRouteWebApplicationAttribute : BaseActionFilterAttribute
{
    private readonly string _appIdKey;
    private readonly string _routeIdKey;
    public ValidateRouteWebApplicationAttribute(string appIdKey, string routeIdKey)
    {
        _appIdKey = appIdKey;
        _routeIdKey = routeIdKey;
    }
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        int appId;
        int routeId;
        if (BaseActionFilterAttribute.TryGetActionArgumentValue<int>(context, _appIdKey, out appId) ||
            BaseActionFilterAttribute.TryGetRouteDataValue<int>(context, _appIdKey, out appId))
        {
            
        }

        if (BaseActionFilterAttribute.TryGetActionArgumentValue<int>(context, _routeIdKey, out routeId) ||
            BaseActionFilterAttribute.TryGetRouteDataValue<int>(context, _routeIdKey, out routeId))
        {
            
        }
        
        if (appId is > 0 && routeId is > 0)
        {
            var webApplicationRouteRepository = context.HttpContext.RequestServices.GetService<IWebApplicationRouteRepository>();
            var route = await webApplicationRouteRepository.Get(routeId);
            if (route == null || route.WebApplicationId != appId)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
                return;
            }
            await base.OnActionExecutionAsync(context, next);
        }
        else
        {
            context.Result = new BadRequestObjectResult(context.ModelState);
            return;
        }
    }
}