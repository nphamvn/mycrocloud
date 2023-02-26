namespace MockServer.Api.TinyFramework;

public interface IAuthenticationHandler
{
    Task<AuthenticateResult> AuthenticateAsync(HttpContext context);
}
