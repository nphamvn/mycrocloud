using Microsoft.AspNetCore.Authentication;

namespace MockServer.Core.Models.Auth;

public class AppAuthenticateResult : AuthenticateResult
{
    public new bool Succeeded { get; set; }
    public new AuthenticationTicket? Ticket { get; set; }
}
