using MockServer.Core.WebApplications;
using MockServer.Core.Identity;
using MockServer.Web.Models.WebApplications.Authentications;
using Route = MockServer.Core.WebApplications.Route;
namespace MockServer.Web.Models.WebApplications;

public class WebApplication
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public WebApplicationAccessibility Accessibility { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public User User { get; set; }
    public IEnumerable<AuthenticationScheme> AuthenticationSchemes { get; set; }
    public IEnumerable<Route> Routes { get; set; }
}