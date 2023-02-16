using MockServer.Core.Enums;

namespace MockServer.Core.Models.Auth;

public class AuthenticationScheme : BaseEntity
{
    public string SchemeName { get; set; }
    public AuthenticationType Type { get; set; }
    public int Order { get; set; }
    public AuthenticationSchemeOptions Options { get; set; }
    public string Description { get; set; }
}
