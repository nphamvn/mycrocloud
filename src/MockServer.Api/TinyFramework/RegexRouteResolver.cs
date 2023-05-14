using System.Text.RegularExpressions;

namespace MockServer.Api.TinyFramework;

public class RegexRouteResolver : IRouteResolver
{
    private readonly ICollection<Route> _routes;

    public RegexRouteResolver(ICollection<Route> routes)
    {
        _routes = routes;
    }
    public async Task<RouteResolveResult> Resolve(string method, string path)
    {
        var routeTemplates = new Dictionary<string, int>();

        if (routeTemplates.TryGetValue(path, out int id))
        {
            return new RouteResolveResult
            {
                Route = _routes.FirstOrDefault(r => r.Id == id)
            };
        }
        else
        {
            foreach (var route in routeTemplates)
            {
                var pattern = route.Key.Replace("{", "(?<").Replace("}", ">.*)");
                // check if the path matches the route template using a regular expression
                var match = Regex.Match(path, pattern);
                if (match.Success)
                {
                    var result = new RouteResolveResult
                    {
                        Route = _routes.FirstOrDefault(r => r.Id == route.Value)
                    };

                    // Extract the route parameter names and values
                    var groups = match.Groups;
                    for (int i = 1; i < groups.Count; i++)
                    {
                        var group = groups[i];
                        if (group.Name != "0")
                        {
                            result.RouteValues[group.Name] = group.Value;
                        }
                    }
                    return result;
                }
            }
            //No matching route found
            return default;
        }
    }
}
