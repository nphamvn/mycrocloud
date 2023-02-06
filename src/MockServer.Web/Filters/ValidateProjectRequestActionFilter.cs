using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MockServer.Core.Repositories;

namespace MockServer.Web.Filters;

public class ValidateProjectRequestAttribute : ActionFilterAttribute
{
    private readonly string ProjectIdKey;
    private readonly string RequestIdKey;
    public ValidateProjectRequestAttribute(string projectIdKey, string requestIdKey)
    {
        ProjectIdKey = projectIdKey;
        RequestIdKey = requestIdKey;
    }
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        int? projectId = null;
        int? requestId = null;
        if (context.ActionArguments.ContainsKey(ProjectIdKey))
        {
            projectId = (int)context.ActionArguments[ProjectIdKey];
        }
        else if (context.RouteData.Values.ContainsKey(ProjectIdKey))
        {
            projectId = Convert.ToInt32((string)context.RouteData.Values[ProjectIdKey]);
        }

        if (context.ActionArguments.ContainsKey(RequestIdKey))
        {
            requestId = (int)context.ActionArguments[RequestIdKey];
        }
        else if (context.RouteData.Values.ContainsKey(RequestIdKey))
        {
            requestId = Convert.ToInt32((string)context.RouteData.Values[RequestIdKey]);
        }
        if (projectId is > 0 && requestId is > 0)
        {
            var requestRepository = context.HttpContext.RequestServices.GetService<IRequestRepository>();
            var request = await requestRepository.GetById(requestId.Value);
            if (request == null || request.ProjectId != projectId)
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