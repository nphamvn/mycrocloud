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
        private readonly IRoutesCreator _routesCreator;
        private readonly IWebApplicationRouteRepository _webApplicationRouteRepository;
        private readonly MockResponderOptions _mockResponderOptions;
        public DownstreamRouteFinderMiddleware(RequestDelegate next,
            IOcelotLoggerFactory loggerFactory,
            IDownstreamRouteProviderFactory downstreamRouteFinder,
            IRoutesCreator routesCreator,
            IWebApplicationRouteRepository webApplicationRouteRepository,
            IOptions<MockResponderOptions> mockResponderOptions
            )
                : base(loggerFactory.CreateLogger<DownstreamRouteFinderMiddleware>())
        {
            _next = next;
            _factory = downstreamRouteFinder;
            _routesCreator = routesCreator;
            _webApplicationRouteRepository = webApplicationRouteRepository;
            _mockResponderOptions = mockResponderOptions.Value;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var upstreamUrlPath = httpContext.Request.Path.ToString();

            var upstreamQueryString = httpContext.Request.QueryString.ToString();

            var upstreamHost = httpContext.Request.Headers["Host"];

            Logger.LogDebug($"Upstream url path is {upstreamUrlPath}");

            var provider = _factory.Get(nameof(DownstreamRouteFinder));

            var app = httpContext.Items.WebApplication();

            var applicableRoutes = await _webApplicationRouteRepository.GetByApplicationId(app.WebApplicationId, string.Empty, string.Empty);
            var routes = _routesCreator.Create(applicableRoutes);
            var response = await provider.Get(upstreamUrlPath, upstreamQueryString, httpContext.Request.Method, routes);
            if (response.IsError)
            {
                Logger.LogWarning($"{MiddlewareName} setting pipeline errors. IDownstreamRouteFinder returned {response.Errors.ToErrorString()}");
                httpContext.Items.UpsertErrors(response.Errors);
                return;
            }
            await SetUpRoute(upstreamUrlPath, upstreamQueryString, httpContext.Request.Method, response.Data.Route);
            var downstreamPathTemplates = string.Join(", ", response.Data.Route.DownstreamRoute.Select(r => r.DownstreamPathTemplate.Value));
            Logger.LogDebug($"downstream templates are {downstreamPathTemplates}");

            // why set both of these on HttpContext
            httpContext.Items.UpsertTemplatePlaceholderNameAndValues(response.Data.TemplatePlaceholderNameAndValues);

            httpContext.Items.UpsertDownstreamRoute(response.Data);

            await _next.Invoke(httpContext);
        }
        private async Task SetUpRoute(string path, string query, string method, Route route)
        {
            route.DownstreamRoute = await BuildDownstreamRoute(path, query, method, route);
        }
        private async Task<List<DownstreamRoute>> BuildDownstreamRoute(string path, string query, string method, Route route)
        {
            var downstreamRoutes = new List<DownstreamRoute>();
            var builder = new DownstreamRouteBuilder();
            switch (route.ResponseProvider)
            {
                case ResponseProvider.Mock:
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
                    break;
                case ResponseProvider.ProxiedServer:
                    var options = new
                    {
                        Path = "Path",
                        ClaimsToHeaders = new List<ClaimToThing>(),
                        ClaimsToClaims = new List<ClaimToThing>(),
                        ClaimsToQueries = new List<ClaimToThing>(),
                        ClaimsToDownstreamPath = new List<ClaimToThing>(),
                        Scheme = "",
                        Host = "",
                        Port = 80,
                        HttpVersion = "",
                        HttpMethod = ""
                    };
                    builder
                        //.WithKey(fileRoute.Key)
                        .WithDownstreamPathTemplate(options.Path)
                        //.WithUpstreamHttpMethod(fileRoute.UpstreamHttpMethod)
                        //.WithUpstreamPathTemplate(upstreamTemplatePattern)
                        //.WithIsAuthenticated(fileRouteOptions.IsAuthenticated)
                        //.WithAuthenticationOptions(authOptionsForRoute)
                        .WithClaimsToHeaders(options.ClaimsToHeaders)
                        .WithClaimsToClaims(options.ClaimsToClaims)
                        //.WithRouteClaimsRequirement(fileRoute.RouteClaimsRequirement)
                        //.WithIsAuthorized(fileRouteOptions.IsAuthorized)
                        .WithClaimsToQueries(options.ClaimsToQueries)
                        .WithClaimsToDownstreamPath(options.ClaimsToDownstreamPath)
                        //.WithRequestIdKey(requestIdKey)
                        //.WithIsCached(fileRouteOptions.IsCached)
                        //.WithCacheOptions(new CacheOptions(fileRoute.FileCacheOptions.TtlSeconds, region))
                        .WithDownstreamScheme(options.Scheme)
                        //.WithLoadBalancerOptions(lbOptions)
                        .WithDownstreamAddresses(new() { new(options.Host, options.Port) })
                        //.WithLoadBalancerKey(routeKey)
                        //.WithQosOptions(qosOptions)
                        //.WithEnableRateLimiting(fileRouteOptions.EnableRateLimiting)
                        //.WithRateLimitOptions(rateLimitOption)
                        //.WithHttpHandlerOptions(httpHandlerOptions)
                        //.WithServiceName(fileRoute.ServiceName)
                        //.WithServiceNamespace(fileRoute.ServiceNamespace)
                        //.WithUseServiceDiscovery(fileRouteOptions.UseServiceDiscovery)
                        //.WithUpstreamHeaderFindAndReplace(hAndRs.Upstream)
                        //.WithDownstreamHeaderFindAndReplace(hAndRs.Downstream)
                        //.WithDelegatingHandlers(fileRoute.DelegatingHandlers)
                        //.WithAddHeadersToDownstream(hAndRs.AddHeadersToDownstream)
                        //.WithAddHeadersToUpstream(hAndRs.AddHeadersToUpstream)
                        //.WithDangerousAcceptAnyServerCertificateValidator(fileRoute.DangerousAcceptAnyServerCertificateValidator)
                        //.WithSecurityOptions(securityOptions)
                        .WithDownstreamHttpVersion(new Version(int.Parse(options.HttpVersion.Split("/")[0]), int.Parse(options.HttpVersion.Split("/")[1])))
                        .WithDownStreamHttpMethod(options.HttpMethod)
                        ;
                    break;
                default:
                    throw new NotSupportedException(nameof(route.ResponseProvider));
            }
            downstreamRoutes.Add(builder.Build());
            return downstreamRoutes;
        }
    }
}
