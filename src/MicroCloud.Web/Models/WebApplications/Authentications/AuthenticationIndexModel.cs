
namespace MicroCloud.Web.Models.WebApplications.Authentications;

public class AuthenticationIndexModel
{
    public AuthenticationIndexModel()
    {
        SelectedAuthenticationSchemeIds = new List<int>();
    }
    public WebApplication WebApplication { get; set; }
    public IEnumerable<AuthenticationSchemeIndexItem> AuthenticationSchemes { get; set; }
    public IEnumerable<int> SelectedAuthenticationSchemeIds { get; set; }
}