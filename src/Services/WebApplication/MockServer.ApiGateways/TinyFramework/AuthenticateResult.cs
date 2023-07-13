using Microsoft.AspNetCore.Authentication;

namespace MockServer.Api.TinyFramework;

public class AuthenticateResult : Microsoft.AspNetCore.Authentication.AuthenticateResult
{
    public bool Succeeded => Ticket != null;
    public AuthenticationTicket? Ticket { get; private set; }
    public Exception? Failure { get; protected set; }
    public static AuthenticateResult Fail(string failureMessage)
    {
        return new AuthenticateResult { Failure = new Exception(failureMessage) };
    }

    public static AuthenticateResult Success(AuthenticationTicket ticket)
    {
        ArgumentNullException.ThrowIfNull(ticket);
        return new AuthenticateResult { Ticket = ticket };
    }
}
