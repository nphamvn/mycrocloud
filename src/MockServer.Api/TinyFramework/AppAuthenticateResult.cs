using Microsoft.AspNetCore.Authentication;

namespace MockServer.Api.TinyFramework;

public class AuthenticateResult : Microsoft.AspNetCore.Authentication.AuthenticateResult
{
    public new bool Succeeded { get; set; }
    public new AuthenticationTicket? Ticket { get; set; }
}
