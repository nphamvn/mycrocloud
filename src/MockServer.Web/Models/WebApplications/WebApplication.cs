using MockServer.Domain.WebApplications;
using MockServer.Domain.Identity;
using MockServer.Web.Models.WebApplications.Authentications;

namespace MockServer.Web.Models.WebApplications;

public class WebApplication
{
    public int WebApplicationId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public WebApplicationAccessibility Accessibility { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public User User { get; set; }
    public IEnumerable<AuthenticationSchemeViewModel> AuthenticationSchemes { get; set; }
    public IEnumerable<Route> Routes { get; set; }
}