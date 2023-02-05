using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MockServer.Core.Repositories;

namespace MockServer.Web.Filters;

public class ValidateProjectRequestAttribute : ActionFilterAttribute
{
    private readonly string ProjectId;
    private readonly string RequestId;
    public ValidateProjectRequestAttribute(string projectId, string requestId)
    {
        ProjectId = projectId;
        RequestId = requestId;
    }
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        int? projectId = null;
        int? requestId = null;
        if (context.ActionArguments.ContainsKey(ProjectId))
        {
            projectId = (int)context.ActionArguments[ProjectId];
        }
        else if (context.RouteData.Values.ContainsKey(ProjectId))
        {
            projectId = Convert.ToInt32((string)context.RouteData.Values[ProjectId]);
        }

        if (context.ActionArguments.ContainsKey(RequestId))
        {
            requestId = (int)context.ActionArguments[RequestId];
        }
        else if (context.RouteData.Values.ContainsKey(RequestId))
        {
            requestId = Convert.ToInt32((string)context.RouteData.Values[RequestId]);
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