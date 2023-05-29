using Ocelot.Configuration;
using Ocelot.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ocelot.DownstreamRouteFinder.Finder
{
    public interface IDownstreamRouteProvider
    {
        //Response<DownstreamRouteHolder> Get(string upstreamUrlPath, string upstreamQueryString, string upstreamHttpMethod, IInternalConfiguration configuration, string upstreamHost);
        Task<Response<DownstreamRouteHolder>> Get(string upstreamUrlPath, string upstreamQueryString, string upstreamHttpMethod, List<Route> routes);
    }
}
