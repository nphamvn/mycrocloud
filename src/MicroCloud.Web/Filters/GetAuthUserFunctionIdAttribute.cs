using Microsoft.AspNetCore.Mvc.Filters;
using WebApplication.Domain.Identity;
using WebApplication.Domain.Repositories;
using MicroCloud.Web.Extentions;

namespace MicroCloud.Web.Filters;

public class GetAuthUserFunctionIdAttribute : ActionFilterAttribute
{
    private readonly string _projectNameKey;
    private readonly string _projectIdKey;
    public GetAuthUserFunctionIdAttribute(string projectName, string projectId)
    {
        _projectNameKey = projectName;
        _projectIdKey = projectId;
    }
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User.ToIdentityUser();
        var projectRepository = context.HttpContext.RequestServices.GetService<IWebApplicationRepository>();
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
            var project = await projectRepository.FindByUserId(user.Id, projectName);
            if (project != null)
            {
                context.ActionArguments[_projectIdKey] = project.Id;
            }
        }
        await base.OnActionExecutionAsync(context, next);
    }
}
