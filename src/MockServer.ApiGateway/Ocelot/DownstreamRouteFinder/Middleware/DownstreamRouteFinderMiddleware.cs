namespace Ocelot.DownstreamRouteFinder.Middleware
{
    using Microsoft.AspNetCore.Http;
    using MockServer.Core.Repositories;
    using Ocelot.Configuration;
    using Ocelot.DownstreamRouteFinder.Finder;
    using Ocelot.Infrastructure.Extensions;
    using Ocelot.Logging;
    using Ocelot.Middleware;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class DownstreamRouteFinderMiddleware : OcelotMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDownstreamRouteProviderFactory _factory;
        private readonly IWebApplicationRouteRepository _webApplicationRouteRepository;

        public DownstreamRouteFinderMiddleware(RequestDelegate next,
            IOcelotLoggerFactory loggerFactory,
            IDownstreamRouteProviderFactory downstreamRouteFinder,
            IWebApplicationRouteRepository webApplicationRouteRepository
            )
                : base(loggerFactory.CreateLogger<DownstreamRouteFinderMiddleware>())
        {
            _next = next;
            _factory = downstreamRouteFinder;
            _webApplicationRouteRepository = webApplicationRouteRepository;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var upstreamUrlPath = httpContext.Request.Path.ToString();

            Logger.LogDebug($"Upstream url path is {upstreamUrlPath}");

            var provider = _factory.Get();

            var app = httpContext.Items.WebApplication();

            var applicableRoutes = await _webApplicationRouteRepository.GetByApplicationId(app.WebApplicationId, string.Empty, string.Empty);

            var response = provider.Get(httpContext.Request.Method, httpContext.Request.Path, Map(applicableRoutes));

            if (response.IsError)
            {
                Logger.LogWarning($"{MiddlewareName} setting pipeline errors. IDownstreamRouteFinder returned {response.Errors.ToErrorString()}");

                httpContext.Items.UpsertErrors(response.Errors);
                return;
            }
            await _next.Invoke(httpContext);
        }

        private List<Route> Map(IEnumerable<MockServer.Core.WebApplications.Route> routes)
        {
            return routes.Select(r => new Route
            {
                RouteId = r.RouteId,
                Method = r.Method,
                RouteTemplate = r.Path
            }).ToList();
        }
    }
}
