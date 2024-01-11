using System.Text.Json;
using Microsoft.AspNetCore.Routing.Template;
using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;
using Route = WebApp.Domain.Entities.Route;

namespace WebApp.MiniApiGateway;

public class RouteResolverMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, IRouteRepository routeRepository, ILogRepository logRepository)
    {
            var foundApp = (App)context.Items["_App"]!;
            var routes = await routeRepository.List(foundApp.Id, "", "");
            var matchedRoutes = new List<Route>();
            foreach (var r in routes)
            {
                var matcher = new TemplateMatcher(TemplateParser.Parse(r.Path), []);
                if (matcher.TryMatch(context.Request.Path, context.Request.RouteValues) && 
                    (r.Method.Equals("ANY") || context.Request.Method.Equals(r.Method, StringComparison.OrdinalIgnoreCase)))
                {
                    matchedRoutes.Add(r);
                }
            }
        
            switch (matchedRoutes.Count)
            {
                case 0:
                    context.Response.StatusCode = 404;
                    return;
                case > 1:
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("The request matched multiple endpoints");
                    return;
            }
        
            var route = matchedRoutes.First();
            context.Items["_Route"] = route;
            object? reqBody = null;
            try
            {
                //TODO:
                reqBody = JsonSerializer.Deserialize<Dictionary<string, object>>(
                    await new StreamReader(context.Request.Body).ReadToEndAsync());
            }
            catch (Exception)
            {
                // ignored
            }
            var request = new
            {
                method = context.Request.Method,
                path = context.Request.Path.Value,
                @params = context.Request.RouteValues.ToDictionary(x => x.Key, x => x.Value?.ToString()),
                query = context.Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString()),
                headers = context.Request.Headers.ToDictionary(x => x.Key, x => x.Value.ToString()),
                body = reqBody
            };
            context.Items["_Request"] = request;
            await next(context);
    }
}

public static class RouteResolverMiddlewareExtensions
{
    public static IApplicationBuilder UseRouteResolverMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RouteResolverMiddleware>();
    }
}