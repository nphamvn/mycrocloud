using Microsoft.AspNetCore.Routing.Template;
using MockServer.Core.Repositories;
using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Services;

public class TemplateParserMatcherRouteService : IRouteResolver
{
    private readonly ICacheService _cacheService;
    private readonly IRequestRepository _requestRepository;
    public TemplateParserMatcherRouteService(ICacheService cacheService, IRequestRepository requestRepository)
    {
        _cacheService = cacheService;
        _requestRepository = requestRepository;
    }

    public async Task<RouteResolveResult> Resolve(string method, string path, ICollection<AppRoute> routes)
    {
        if (!path.StartsWith("/"))
        {
            path = "/" + path;
        }
        int matchCount = 0;
        foreach (var route in routes)
        {
            RouteTemplate template = TemplateParser.Parse(route.Path);
            TemplateMatcher matcher = new TemplateMatcher(template, new RouteValueDictionary());
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
        return default;
    }
}
