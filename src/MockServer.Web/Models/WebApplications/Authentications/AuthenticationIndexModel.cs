
namespace MockServer.Web.Models.WebApplications.Authentications;

public class AuthenticationIndexModel
{
    public WebApplication WebApplication { get; set; }
    public IEnumerable<AuthenticationSchemeIndexItem> AuthenticationSchemes { get; set; }
}