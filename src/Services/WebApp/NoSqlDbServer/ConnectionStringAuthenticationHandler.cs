using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace NoSqlDbServer;

public class ConnectionStringAuthenticationHandler
    (IOptionsMonitor<ConnectionStringAuthenticationSchemOptions> options,
     ILoggerFactory logger,
     UrlEncoder encoder)
    : AuthenticationHandler<ConnectionStringAuthenticationSchemOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var connectionString = Request.Headers["X-Connection-String"];
        Console.WriteLine("X-Connection-String: " + connectionString);
        if (string.IsNullOrEmpty(connectionString))
        {
            return AuthenticateResult.Fail("Missing X-Connection-String header");
        }
        //TODO: validate connection string
        const int databaseId = 1;
        var claims = new[] { new Claim("DataSource", databaseId.ToString()) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}
