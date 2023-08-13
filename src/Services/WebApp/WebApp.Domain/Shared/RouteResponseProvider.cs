using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Domain.Shared;
public enum RouteResponseProvider
{
    [Display(Name = "Mock")]
    [Description("Mock")]
    Mock = 1,
    [Display(Name = "Proxied Server")]
    [Description("The application will send the request to a specified proxied server, fetches the response, and sends it back to the client.")]
    ProxiedServer = 2,
    [Display(Name = "Function")]
    [Description("Function")]
    Function = 3
}
