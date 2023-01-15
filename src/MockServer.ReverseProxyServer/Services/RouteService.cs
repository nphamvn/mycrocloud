using System.Text.RegularExpressions;
using MockServer.Core.Repositories;
using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Services;

public class RouteService : IRouteResolver
{
    private readonly ICacheService _cacheService;
    private readonly IRequestRepository _requestRepository;

    public RouteService(ICacheService cacheService, IRequestRepository requestRepository)
    {
        _cacheService = cacheService;
        _requestRepository = requestRepository;
    }
    public async Task<RouteResolveResult> Resolve(string method, string path, ICollection<AppRoute> routes)
    {
        var routeTemplates = new Dictionary<string, int>();

        if (routeTemplates.TryGetValue(path, out int id))
        {
            return new RouteResolveResult
            {
                Route = routes.FirstOrDefault(r => r.Id == id)
            };
        }
        else
        {
            foreach (var route in routeTemplates)
            {
                //var pattern = route.Key.Replace("{", "(?<").Replace("}", ">.+)");
                var pattern = route.Key.Replace("{", "(?<").Replace("}", ">.*)");
                //"^" + route.Value + "$"
                // check if the path matches the route template using a regular expression
                var match = Regex.Match(path, pattern);
                if (match.Success)
                {
                    var result = new RouteResolveResult
                    {
                        Route = routes.FirstOrDefault(r => r.Id == route.Value)
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
