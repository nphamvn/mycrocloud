
namespace MockServer.Domain.WebApplication.Route;

public class BodyValidationItem
{
    public string Field { get; set; }
    public ICollection<RouteValidationRule> Rules { get; set; }
}
