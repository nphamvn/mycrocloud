using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;

namespace MockServer.Core.Entities.Auth;

public class AppAuthentication : BaseEntity
{
    public string SchemeName { get; set; }
    public AuthenticationType Type { get; set; }
    public int Order { get; set; }
    public AuthOptions Options { get; set; }
    public string Description { get; set; }
}
