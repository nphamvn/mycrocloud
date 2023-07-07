using MockServer.Domain.WebApplication.Shared;

namespace MockServer.Web.Models.WebApplications.Authentications;

public class AuthenticationSchemeIndexItem
{
    public int Id { get; set; }
    public AuthenticationSchemeType Type { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public int Order { get; set; }
    public string Description { get; set; }
}
