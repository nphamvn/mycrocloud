namespace Ocelot.Configuration
{
    using MockServer.Domain.WebApplications;
    using Ocelot.Configuration.File;
    using Ocelot.Values;
    using System.Collections.Generic;
    using System.Net.Http;

    public class Route
    {
        public Route(int id,
            RouteResponseProvider responseProvider,
            List<DownstreamRoute> downstreamRoute,
            List<AggregateRouteConfig> downstreamRouteConfig,
            List<HttpMethod> upstreamHttpMethod,
            UpstreamPathTemplate upstreamTemplatePattern,
            string upstreamHost,
            string aggregator)
        {
            Id = id;
            ResponseProvider = responseProvider;
            UpstreamHost = upstreamHost;
            DownstreamRoute = downstreamRoute;
            DownstreamRouteConfig = downstreamRouteConfig;
            UpstreamHttpMethod = upstreamHttpMethod;
            UpstreamTemplatePattern = upstreamTemplatePattern;
            Aggregator = aggregator;
        }
        public int Id { get; set; }
        public RouteResponseProvider ResponseProvider { get; set; }
        public UpstreamPathTemplate UpstreamTemplatePattern { get; set; }
        public List<HttpMethod> UpstreamHttpMethod { get; private set; }
        public string UpstreamHost { get; private set; }
        public List<DownstreamRoute> DownstreamRoute { get; set; }
        public List<AggregateRouteConfig> DownstreamRouteConfig { get; private set; }
        public string Aggregator { get; private set; }
    }
}