using Microsoft.AspNetCore.Mvc.Filters;
using MockServer.Core.Models;
using MockServer.Core.Repositories;
using MockServer.Web.Extentions;

namespace MockServer.Web.Filters;

public class GetAuthUserProjectIdAttribute : ActionFilterAttribute
{
    private readonly string _projectNameKey;
    private readonly string _projectIdKey;
    public GetAuthUserProjectIdAttribute(string projectName, string projectId)
    {
        _projectNameKey = projectName;
        _projectIdKey = projectId;
    }
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User.Parse<ApplicationUser>();
        var projectRepository = context.HttpContext.RequestServices.GetService<IProjectRepository>();
        string projectName = null;
        if (context.ActionArguments.ContainsKey(_projectNameKey))
        {
            projectName = (string)context.ActionArguments[_projectNameKey];
        }
        else if (context.RouteData.Values.ContainsKey(_projectNameKey))
        {
            projectName = (string)context.RouteData.Values[_projectNameKey];
        }
        if (!string.IsNullOrEmpty(projectName))
        {
            var project = await projectRepository.Find(user.Id, projectName);
            if (project != null)
            {
                context.ActionArguments[_projectIdKey] = project.Id;
            }
        }
        await base.OnActionExecutionAsync(context, next);
    }
}
