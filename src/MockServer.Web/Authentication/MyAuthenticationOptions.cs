using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MockServer.Web.Authentication;
public class MyAuthenticationOptions : AuthenticationSchemeOptions
{
    public ITicketStore? SessionStore { get; set; }
}