using Microsoft.AspNetCore.Mvc.Filters;

namespace MycroCloud.WebMvc.Filters;
public class BaseActionFilterAttribute : ActionFilterAttribute
{
    public static bool TryGetActionArgumentValue<T>(ActionExecutingContext context, string key, out T value)
    {
        if (context.ActionArguments.ContainsKey(key))
        {
            object argumentValue = context.ActionArguments[key];
            if (argumentValue is T)
            {
                value = (T)argumentValue;
                return true;
            }
        }

        value = default(T);
        return false;
    }

    public static bool TryGetRouteDataValue<T>(ActionExecutingContext context, string key, out T value)
    {
        if (context.RouteData.Values.TryGetValue(key, out object result) && result is T)
        {
            value = (T)result;
            return true;
        }

        value = default;
        return false;
    }
}