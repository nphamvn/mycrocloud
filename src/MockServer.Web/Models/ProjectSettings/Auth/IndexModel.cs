using MockServer.Core.Entities.Auth;

namespace MockServer.Web.Models.ProjectSettings.Auth;

public class AuthIndexModel : ProjectSettingsBaseModel
{
    //public IEnumerable<AppAuthentication> AllAuthenticationSchemes { get; set; }
    public IEnumerable<AppAuthentication> AuthenticationSchemes { get; set; }
}
