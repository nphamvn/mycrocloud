namespace Ocelot.DownstreamRouteFinder
{
    using Microsoft.AspNetCore.Routing;
    using Ocelot.Configuration;
    using Ocelot.DownstreamRouteFinder.UrlMatcher;
    using Route = Ocelot.Configuration.Route;
    using System.Collections.Generic;

    public class DownstreamRouteHolder
    {
        public DownstreamRouteHolder()
        {

        }
        public DownstreamRouteHolder(List<PlaceholderNameAndValue> templatePlaceholderNameAndValues, Route route)
        {
            //TemplatePlaceholderNameAndValues = templatePlaceholderNameAndValues;
            Route = route;
        }

        //public List<PlaceholderNameAndValue> TemplatePlaceholderNameAndValues { get; private set; }
        public Route Route { get; set; }
        public RouteValueDictionary RoutValues { get; set; }
    }
}
