using Microsoft.AspNetCore.Routing.Template;

namespace MockServer.Api.TinyFramework;

public class TemplateParserMatcherRouteService : IRouteResolver
{
    private readonly ICollection<Route> _routes;

    public TemplateParserMatcherRouteService(ICollection<Route> routes)
    {
        this._routes = routes;
    }
    public async Task<RouteResolveResult> Resolve(string method, string path)
    {
        //ensure that path is ended with slash
        path = $"{path.TrimEnd('/')}/";
        int matchCount = 0;
        foreach (var route in _routes)
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
