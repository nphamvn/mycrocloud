using MockServer.Core.Entities.Auth;

namespace MockServer.WebMVC.Models.ProjectSettings.Auth;

public class AuthIndexModel : ProjectSettingsBaseModel
{
    public IEnumerable<AppAuthentication> Authentications { get; set; }
}
