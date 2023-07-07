
namespace MockServer.Domain.WebApplication.Route;

public class HeaderValidationItem
{
    public string Name { get; set; }
    public ICollection<RouteValidationRule> Rules { get; set; }
}
