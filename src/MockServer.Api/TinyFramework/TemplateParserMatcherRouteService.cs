using Microsoft.AspNetCore.Routing.Template;
using MockServer.Core.Repositories;
using MockServer.Api.Interfaces;

namespace MockServer.Api.TinyFramework;

public class TemplateParserMatcherRouteService : IRouteResolver
{
    private readonly ICacheService _cacheService;
    private readonly IWebApplicationRouteRepository _requestRepository;
    
    public TemplateParserMatcherRouteService(ICacheService cacheService, 
        IWebApplicationRouteRepository requestRepository)
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
            var template = TemplateParser.Parse(route.RouteTemplate);
            var matcher = new TemplateMatcher(template, new RouteValueDictionary());
            var values = new RouteValueDictionary();
            var match = matcher.TryMatch(new PathString(path), values);
            if (match && (route.Method.Equals(method) || route.Method.Equals("*")))
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
