using Microsoft.AspNetCore.Mvc.Filters;

namespace MycroCloud.WebMvc.Filters;
public class BaseActionFilterAttribute : ActionFilterAttribute
{
    public static bool TryGetActionArgumentValue<T>(ActionExecutingContext context, string key, out T value)
    {
        if (context.ActionArguments.TryGetValue(key, out object argumentValue))
        {
            if (argumentValue is T t)
            {
                value = t;
                return true;
            }
        }

        value = default;
        return false;
    }

    public static bool TryGetRouteDataValue<T>(ActionExecutingContext context, string key, out T value)
    {
        if (context.RouteData.Values.TryGetValue(key, out object result) && result is T t)
        {
            value = t;
            return true;
        }

        value = default;
        return false;
    }
}