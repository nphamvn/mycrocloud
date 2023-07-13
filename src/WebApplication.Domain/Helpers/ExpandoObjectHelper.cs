using System.Dynamic;

namespace WebApplication.Domain.Extentions;

public static class ExpandoObjectHelper
{
    public static bool PropertyExist(dynamic obj, string property)
    {
        if (obj is ExpandoObject)
            return ((IDictionary<string, object>)obj).ContainsKey(property);

        return obj.GetType().GetProperty(property) != null;
    }
}
