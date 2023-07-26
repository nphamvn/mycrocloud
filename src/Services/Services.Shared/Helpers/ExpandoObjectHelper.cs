using System.Dynamic;

namespace  MycroCloud.Shared.Helpers;

public static class ExpandoObjectHelper
{
    public static bool PropertyExist(dynamic obj, string property)
    {
        if (obj is ExpandoObject)
            return ((IDictionary<string, object>)obj).ContainsKey(property);

        return obj.GetType().GetProperty(property) != null;
    }
}
