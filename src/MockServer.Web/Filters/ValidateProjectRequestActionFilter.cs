using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MockServer.Core.Repositories;

namespace MockServer.Web.Filters;

public class ValidateProjectRequestAttribute : ActionFilterAttribute
{
    private readonly string _projectIdKey;
    private readonly string _requestIdKey;
    public ValidateProjectRequestAttribute(string projectIdKey, string requestIdKey)
    {
        _projectIdKey = projectIdKey;
        _requestIdKey = requestIdKey;
    }
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        int? projectId = null;
        int? requestId = null;
        if (context.ActionArguments.ContainsKey(_projectIdKey))
        {
            projectId = (int)context.ActionArguments[_projectIdKey];
        }
        else if (context.RouteData.Values.ContainsKey(_projectIdKey))
        {
            projectId = Convert.ToInt32((string)context.RouteData.Values[_projectIdKey]);
        }

        if (context.ActionArguments.ContainsKey(_requestIdKey))
        {
            requestId = (int)context.ActionArguments[_requestIdKey];
        }
        else if (context.RouteData.Values.ContainsKey(_requestIdKey))
        {
            requestId = Convert.ToInt32((string)context.RouteData.Values[_requestIdKey]);
        }
        if (projectId is > 0 && requestId is > 0)
        {
            var requestRepository = context.HttpContext.RequestServices.GetService<IWebApplicationRouteRepository>();
            var request = await requestRepository.GetById(requestId.Value);
            if (request == null || request.ApplicationId != projectId)
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