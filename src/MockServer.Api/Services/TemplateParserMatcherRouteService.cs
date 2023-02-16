using Microsoft.AspNetCore.Routing.Template;
using MockServer.Core.Repositories;
using MockServer.Api.Interfaces;
using Route = MockServer.Api.Models.Route;

namespace MockServer.Api.Services;

public class TemplateParserMatcherRouteService : IRouteResolver
{
    private readonly ICacheService _cacheService;
    private readonly IRequestRepository _requestRepository;
    public TemplateParserMatcherRouteService(ICacheService cacheService, IRequestRepository requestRepository)
    {
        _cacheService = cacheService;
        _requestRepository = requestRepository;
    }

    public async Task<RouteResolveResult> Resolve(string method, string path, ICollection<Route> routes)
    {
        if (!path.StartsWith("/"))
        {
            path = "/" + path;
        }
        int matchCount = 0;
        foreach (var route in routes)
        {
            var template = TemplateParser.Parse(route.Path);
            var matcher = new TemplateMatcher(template, new RouteValueDictionary());
            var values = new RouteValueDictionary();
            var match = matcher.TryMatch(new PathString(path), values);
            if (match && route.Method.Equals(method))
            {
                matchCount++;
                return new RouteResolveResult
                {
                    Route = route,
                    RouteValues = values
                };
            }
        }
        return null;
    }
}
