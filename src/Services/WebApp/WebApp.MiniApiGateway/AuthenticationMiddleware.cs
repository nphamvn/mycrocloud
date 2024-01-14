using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using WebApp.Domain.Entities;
using WebApp.Domain.Enums;
using WebApp.Domain.Repositories;

namespace WebApp.MiniApiGateway;

public class AuthenticationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var app = (App)context.Items["_App"]!;
        var appRepository = context.RequestServices.GetService<IAppRepository>()!;
        var cachedOpenIdConnectionSigningKeys = context.RequestServices.GetService<ICachedOpenIdConnectionSigningKeys>()!;
        var authenticationSchemes = await appRepository.GetAuthenticationSchemes(app.Id);
        if (!authenticationSchemes.Any())
        {
            await next.Invoke(context);
            return;
        }
        foreach (var scheme in authenticationSchemes)
        {
            //TODO: Get from settings
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                continue;
            }

            if (scheme.Type == AuthenticationSchemeType.OpenIdConnect)
            {
                var signingKeys = await cachedOpenIdConnectionSigningKeys.Get(scheme.OpenIdConnectAuthority);
                if (!ValidateToken(token, scheme.OpenIdConnectAuthority,
                        scheme.OpenIdConnectAudience,
                        signingKeys, out var jwt))  
                {
                    continue;
                }
                //TODO: 
                var claims = jwt.Claims;
                var user = new Dictionary<string, string>();
                foreach (var claim in claims)
                {
                    //TODO: A claim can have multiple values
                    if (!user.TryAdd(claim.Type, claim.Value))
                    {
                        user[claim.Type] = claim.Value;
                    }
                }
                context.Items.Add("_AuthenticatedScheme", scheme);
                context.Items.Add("_OpenIdConnectUser", user);
            }
        }
        await next.Invoke(context);
    }
    private static bool ValidateToken(string token, 
        string issuer, 
        string audience, 
        ICollection<SecurityKey> signingKeys,
        out JwtSecurityToken jwt)
    {
        var validationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = signingKeys,
            ValidateLifetime = true
        };
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
            jwt = (JwtSecurityToken)validatedToken;
    
            return true;
        } catch (SecurityTokenValidationException ex) {
            // Log the reason why the token is not valid
            jwt = null;
            return false;
        }
    }
}
public static class AuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthenticationMiddleware>();
    }
}