namespace Ocelot.DownstreamRouteFinder
{
    using Ocelot.DownstreamRouteFinder.UrlMatcher;
    using Route = Ocelot.Configuration.Route;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Routing;

    public class DownstreamRouteHolder
    {
        public DownstreamRouteHolder(Route route, RouteValueDictionary routValues)
            : this(Map(routValues), route)
        {

        }
        public DownstreamRouteHolder(List<PlaceholderNameAndValue> templatePlaceholderNameAndValues, Route route)
        {
            TemplatePlaceholderNameAndValues = templatePlaceholderNameAndValues;
            Route = route;
        }
        private static List<PlaceholderNameAndValue> Map(RouteValueDictionary routeValues)
        {
            var placeholders = new List<PlaceholderNameAndValue>();
    
            foreach (var item in routeValues)
            {
                var placeholder = new PlaceholderNameAndValue(item.Key, item.Value?.ToString());
                placeholders.Add(placeholder);
            }
            
            return placeholders;
        }
        public List<PlaceholderNameAndValue> TemplatePlaceholderNameAndValues { get; private set; }
        public Route Route { get; set; }
    }
}
