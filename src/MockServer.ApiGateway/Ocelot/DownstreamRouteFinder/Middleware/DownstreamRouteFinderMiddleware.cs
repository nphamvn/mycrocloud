namespace Ocelot.DownstreamRouteFinder.Middleware
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Options;
    using MockServer.Core.Repositories;
    using MockServer.Core.WebApplications;
    using Ocelot.Configuration;
    using Ocelot.Configuration.Builder;
    using Ocelot.Configuration.Creator;
    using Ocelot.DownstreamRouteFinder.Finder;
    using Ocelot.Infrastructure.Extensions;
    using Ocelot.Logging;
    using Ocelot.Middleware;
    using Ocelot.Values;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Route = Configuration.Route;

    public class DownstreamRouteFinderMiddleware : OcelotMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDownstreamRouteProviderFactory _factory;
        private readonly IWebApplicationRouteRepository _webApplicationRouteRepository;
        private readonly MockResponderOptions _mockResponderOptions;
        public DownstreamRouteFinderMiddleware(RequestDelegate next,
            IOcelotLoggerFactory loggerFactory,
            IDownstreamRouteProviderFactory downstreamRouteFinder,
            IWebApplicationRouteRepository webApplicationRouteRepository,
            IOptions<MockResponderOptions> mockResponderOptions
            )
                : base(loggerFactory.CreateLogger<DownstreamRouteFinderMiddleware>())
        {
            _next = next;
            _factory = downstreamRouteFinder;
            _webApplicationRouteRepository = webApplicationRouteRepository;
            _mockResponderOptions = mockResponderOptions.Value;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var upstreamUrlPath = httpContext.Request.Path.ToString();

            var upstreamQueryString = httpContext.Request.QueryString.ToString();

            var upstreamHost = httpContext.Request.Headers["Host"];

            Logger.LogDebug($"Upstream url path is {upstreamUrlPath}");

            var provider = _factory.Get();

            var app = httpContext.Items.WebApplication();

            var applicableRoutes = await _webApplicationRouteRepository.GetByApplicationId(app.WebApplicationId, string.Empty, string.Empty);

            var response = await provider.Get(upstreamUrlPath, upstreamQueryString, httpContext.Request.Method, Map(applicableRoutes));

            if (response.IsError)
            {
                Logger.LogWarning($"{MiddlewareName} setting pipeline errors. IDownstreamRouteFinder returned {response.Errors.ToErrorString()}");
                httpContext.Items.UpsertErrors(response.Errors);
                return;
            }
            response.Data.Route.DownstreamRoute = await BuildDownstreamRoute(upstreamUrlPath, upstreamQueryString, httpContext.Request.Method, response.Data.Route);
            var downstreamPathTemplates = string.Join(", ", response.Data.Route.DownstreamRoute.Select(r => r.DownstreamPathTemplate.Value));
            Logger.LogDebug($"downstream templates are {downstreamPathTemplates}");

            // why set both of these on HttpContext
            httpContext.Items.UpsertTemplatePlaceholderNameAndValues(response.Data.TemplatePlaceholderNameAndValues);

            httpContext.Items.UpsertDownstreamRoute(response.Data);

            await _next.Invoke(httpContext);
        }
        private async Task<List<DownstreamRoute>> BuildDownstreamRoute(string path, string query, string method, Route route)
        {
            var downstreamRoutes = new List<DownstreamRoute>();
            var builder = new DownstreamRouteBuilder();
            switch (route.ResponseProvider)
            {
                case ResponseProvider.MockResponse:
                    builder.WithAddHeadersToUpstream(new() {
                                                new AddHeader(_mockResponderOptions.RouteIdHeader, route.Id.ToString())
                                            })
                    .WithLoadBalancerKey($"LoadBalancerKey-{route.Id}")
                    .WithLoadBalancerOptions(new LoadBalancerOptions("NoLoadBalancer", $"LoadBalancerKey-{route.Id}", 0))
                    .WithQosOptions(new QoSOptions(3, 5000, 5000, "QoSOptionsKey"))
                    .WithDownstreamHttpVersion(new Version(1, 1))
                    .WithHttpHandlerOptions(new HttpHandlerOptions(false, false, false, false, 1))
                    .WithDownstreamPathTemplate(path)
                    .WithDownstreamScheme(_mockResponderOptions.Scheme)
                    .WithDownstreamAddresses(new() {
                        new (_mockResponderOptions.Host, _mockResponderOptions.Port)
                    });
                    downstreamRoutes.Add(builder.Build());
                    break;
                case ResponseProvider.RequestForward:
                    break;
                default:
                    throw new NotSupportedException(nameof(route.ResponseProvider));
            }
            return downstreamRoutes;
        }

        private List<Route> Map(IEnumerable<MockServer.Core.WebApplications.Route> routes)
        {
            return routes.Select(r =>
            {
                var builder = new RouteBuilder();
                return builder
                    .WithId(r.RouteId)
                    .WithUpstreamHttpMethod(new() { r.Method })
                    .WithUpstreamPathTemplate(new UpstreamPathTemplate(r.Path, 1, false, r.Path))
                    .WithResponseProvider(r.ResponseProvider)
                    .Build();
            })
            .ToList();
        }
    }
}
