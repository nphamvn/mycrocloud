using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace MockServer.Web.Authentication;
public class MyAuthenticationHandler : AuthenticationHandler<MyAuthenticationOptions>, IAuthenticationSignInHandler
{
    public MyAuthenticationHandler(IOptionsMonitor<MyAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    public Task SignInAsync(ClaimsPrincipal user, AuthenticationProperties properties)
    {
            // Create a new authentication ticket with the provided user principal
            var ticket = new AuthenticationTicket(user, Scheme.Name);

            var serializedTicket = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ticket));
            
            Context.Session.Set("MyAuthenticationHandler.AuthenticationTicket",serializedTicket);
            // Return the authentication ticket to the caller
            return Task.CompletedTask;
    }

    public async Task SignOutAsync(AuthenticationProperties properties)
    {
        // call SignOutAsync on the HttpContext using the authentication scheme
        await Context.SignOutAsync(Scheme.Name, properties);
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        byte[] serializedTicket;
        if (!Context.Session.TryGetValue("MyAuthenticationHandler.AuthenticationTicket", out serializedTicket))
        {
            return Task.FromResult(AuthenticateResult.Fail("Session not found"));
        }

        //var ticket = JsonConvert.DeserializeObject<AuthenticationTicket>(Encoding.UTF8.GetString(serializedTicket));

        // create claims array from the model
        var claims = new[] {
            new Claim(ClaimTypes.Email, "pvnam95@hotmail.com")
        };
        // generate claimsIdentity on the name of the class
        var claimsIdentity = new ClaimsIdentity(claims, "MyAuthenticationHandler");
        // generate AuthenticationTicket from the Identity
        // and current authentication scheme
        var ticket = new AuthenticationTicket(
            new ClaimsPrincipal(claimsIdentity), this.Scheme.Name);
        // pass on the ticket to the middleware
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

}