using Microsoft.AspNetCore.Routing;

namespace MockServer.Core.Extentions;

public static class RouteValueDictionaryExtentions
{
    public static RouteValueDictionary With(this RouteValueDictionary dictionary, string key, object value) {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, value);
        }
        else 
        {
            dictionary[key] = value;
        }
        return dictionary;
    }   
}
