using System.Text.RegularExpressions;
using MockServer.Core.Repositories;
using MockServer.ReverseProxyServer.Interfaces;

namespace MockServer.ReverseProxyServer.Services;

public class RouteService : IRouteService
{
    private readonly ICacheService _cacheService;
    private readonly IRequestRepository _requestRepository;

    public RouteService(ICacheService cacheService, IRequestRepository requestRepository)
    {
        _cacheService = cacheService;
        _requestRepository = requestRepository;
    }
    public async Task Map(int projectId)
    {
        string key = projectId.ToString();
        var paths = await _requestRepository.GetProjectRequests(projectId);
        Dictionary<string, int> routes = paths.ToDictionary(r => r.Path, r => r.Id);
        await _cacheService.Set(projectId.ToString(), routes);
    }

    public async Task<RouteResolveResult> Resolve(string path, int projectId)
    {
        if (!await _cacheService.Exists(projectId.ToString()))
        {
            await this.Map(projectId);
        }
        var routes = await _cacheService.Get<Dictionary<string, int>>(projectId.ToString());
        if (routes.TryGetValue(path, out int id))
        {
            return new RouteResolveResult
            {
                RequestId = id
            };
        }
        else
        {
            foreach (var route in routes)
            {
                var pattern = route.Key.Replace("{", "(?<").Replace("}", ">.+)");
                //"^" + route.Value + "$"
                // check if the path matches the route template using a regular expression
                var match = Regex.Match(path, pattern);
                if (match.Success)
                {
                    var result = new RouteResolveResult
                    {
                        RequestId = route.Value
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
