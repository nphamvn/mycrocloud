using MockServer.Core.Services;
using MockServer.Core.WebApplications.Security;

namespace MockServer.Core.WebApplications;

public class WebApplication : Service
{
    public string Description { get; set; }
    public WebApplicationAccessibility Accessibility { get; set; }
    public List<AuthenticationScheme> AuthenticationSchemes { get; set; }
    public List<string> UseMiddlewares { get; set; }
    public bool Blocked { get; set; }
}