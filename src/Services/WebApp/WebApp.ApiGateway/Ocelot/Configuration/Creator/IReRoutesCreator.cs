using Ocelot.Configuration.File;
using System.Collections.Generic;
using WebApp.Domain.Entities;

namespace Ocelot.Configuration.Creator
{
    public interface IRoutesCreator
    {
        List<Route> Create(FileConfiguration fileConfiguration);
        List<Route> Create(IEnumerable<RouteEntity> routes);
    }
}
