using MockServer.Core.Entities.Auth;

namespace MockServer.WebMVC.Models.ProjectSettings.Auth;

public class AuthIndexModel : ProjectSettingsBaseModel
{
    public IEnumerable<Authentication> Authentications { get; set; }
}
