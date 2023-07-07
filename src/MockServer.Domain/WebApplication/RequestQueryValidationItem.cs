
namespace MockServer.Domain.WebApplication.Route;

public class QueryParameterValidationItem
{
    public string Name { get; set; }
    public ICollection<RouteValidationRule> Rules { get; set; }
}
