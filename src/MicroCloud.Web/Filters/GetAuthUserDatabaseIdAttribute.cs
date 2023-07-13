using Microsoft.AspNetCore.Mvc.Filters;
using WebApplication.Domain.Identity;
using WebApplication.Domain.Repositories;
using MicroCloud.Web.Extentions;

namespace MicroCloud.Web.Filters;

public class GetAuthUserDatabaseIdAttribute : ActionFilterAttribute
{
    private readonly string _databaseNameKey;
    private readonly string _databaseIdKey;
    public GetAuthUserDatabaseIdAttribute(string databaseNameKey, string datbaseIdKey)
    {
        _databaseNameKey = databaseNameKey;
        _databaseIdKey = datbaseIdKey;
    }
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User.ToIdentityUser();
        var databaseRepository = context.HttpContext.RequestServices.GetService<IDatabaseRepository>();
        string databaseName = null;
        if (context.ActionArguments.ContainsKey(_databaseNameKey))
        {
            databaseName = (string)context.ActionArguments[_databaseNameKey];
        }
        else if (context.RouteData.Values.ContainsKey(_databaseNameKey))
        {
            databaseName = (string)context.RouteData.Values[_databaseNameKey];
        }
        if (!string.IsNullOrEmpty(databaseName))
        {
            var database = await databaseRepository.FindByUserId(user.Id, databaseName);
            if (database != null)
            {
                context.ActionArguments[_databaseIdKey] = database.Id;
            }
        }
        await base.OnActionExecutionAsync(context, next);
    }
}
