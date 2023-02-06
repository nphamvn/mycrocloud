using Microsoft.AspNetCore.Mvc.Filters;
using MockServer.Core.Models;
using MockServer.Core.Repositories;
using MockServer.Web.Extentions;

namespace MockServer.Web.Filters;

public class GetAuthUserProjectIdAttribute : ActionFilterAttribute
{
    public const string DefaultProjectNameArgument = "ProjectName";
    public const string DefaultProjectIdArgument = "ProjectId";
    private string projectNameArgument;
    private string projectIdArgument;
    public GetAuthUserProjectIdAttribute(string projectName, string projectId)
    {
        projectNameArgument = projectName;
        projectIdArgument = projectId;
    }
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User.Parse<ApplicationUser>();
        var projectRepository = context.HttpContext.RequestServices.GetService<IProjectRepository>();
        string projectName = null;
        if (context.ActionArguments.ContainsKey(projectNameArgument))
        {
            projectName = (string)context.ActionArguments[projectNameArgument];
        }
        else if (context.RouteData.Values.ContainsKey(projectNameArgument))
        {
            projectName = (string)context.RouteData.Values[projectNameArgument];
        }
        if (!string.IsNullOrEmpty(projectName))
        {
            var project = await projectRepository.Find(user.Id, projectName);
            if (project != null)
            {
                context.ActionArguments[projectIdArgument] = project.Id;
            }
        }
        await base.OnActionExecutionAsync(context, next);
    }
}
