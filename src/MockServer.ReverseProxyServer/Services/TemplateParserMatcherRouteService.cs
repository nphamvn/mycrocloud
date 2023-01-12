using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing.Template;
using MockServer.Core.Repositories;
using MockServer.ReverseProxyServer.Interfaces;

namespace MockServer.ReverseProxyServer.Services;

public class TemplateParserMatcherRouteService : IRouteService
{
    private readonly ICacheService _cacheService;
    private readonly IRequestRepository _requestRepository;
    public TemplateParserMatcherRouteService(ICacheService cacheService, IRequestRepository requestRepository)
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
        if (!path.StartsWith("/"))
        {
            path = "/" + path;
        }
        if (!await _cacheService.Exists(projectId.ToString()))
        {
            await this.Map(projectId);
        }
        var routeTemplates = await _cacheService.Get<Dictionary<string, int>>(projectId.ToString());
        foreach (var route in routeTemplates)
        {
            RouteTemplate template = TemplateParser.Parse(route.Key);
            TemplateMatcher matcher = new TemplateMatcher(template, new RouteValueDictionary());
            var values = new RouteValueDictionary();
            var match = matcher.TryMatch(new PathString(path), values);
            if (match)
            {
                return new RouteResolveResult
                {
                    RequestId = route.Value,
                    RouteValues = values
                };
            }
        }
        return default;
    }
}
