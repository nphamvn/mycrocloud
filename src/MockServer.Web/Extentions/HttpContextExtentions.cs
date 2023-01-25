public static class HttpContextExtentions
{
    public static Dictionary<string, object> GetRequestDictionary(HttpContext context)
    {
        var request = context.Request;

        return new Dictionary<string, object>()
            {
                { "method", request.Method},
                { "path", request.Path.ToString()},
                { "host", request.Host.Host},
                { "headers", request.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault())},
                { "routeValues", request.RouteValues.ToDictionary(rv => rv.Key, rv => rv.Value)},
                { "query", request.Query.ToDictionary(q=> q.Key, q => q.Value.FirstOrDefault())}
            };
    }
}