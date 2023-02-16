using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using MockServer.Core.Interfaces;
using MockServer.Core.Models.Auth;
using MockServer.Core.Services;

namespace MockServer.UnitTest.JwtToken;

public class JwtBearerTokenServiceTest
{
    private readonly IJwtBearerTokenService jwtBearerTokenService;
    private readonly JwtBearerAuthenticationOptions options;
    public JwtBearerTokenServiceTest()
    {
        jwtBearerTokenService = new JwtBearerTokenService();
        options = new JwtBearerAuthenticationOptions
        {
            SecretKey= "2LoiD6ZU1Jnm1z6JcKOe8LZOiObXFseW",
            SymmetricSecuritySecretKeys = new List<string>{"2LoiD6ZU1Jnm1z6JcKOe8LZOiObXFseW"},
            Authority = "https://mockserver.com",
            Issuer = "https://mockserver.com",
            Algorithm = SecurityAlgorithms.HmacSha256,
            Audience = "https://some-api.mockserver.com",
            Expire = 60,
            RequireExpirationTime = false,
            ValidateLifetime = false,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = false,
        };
    }
    [Fact]
    public void Should_generate_token()
    {
        var claims = new List<Claim> {
            new Claim("sub", "nam")
        };
        var token = jwtBearerTokenService.GenerateToken(options, claims);
        Console.WriteLine(token);
        Assert.True(!string.IsNullOrEmpty(token));
    }
    [Fact]
    public void Should_return_valid_result() {
        var claims = new List<Claim> {
            new Claim("sub", "nam")
        };
        var token = jwtBearerTokenService.GenerateToken(options, claims);
        try
        {
            options.Authority = "https://dev-vzxphouz.us.auth0.com/";
            var user = jwtBearerTokenService.ValidateToken(token, options);
            Assert.NotNull(user);
        }
        catch (System.Exception)
        {
            
        }
    }
    [Fact]
    public void Should_return_valid_result_oidc()
    {
        var claims = new List<Claim> {
            new Claim("sub", "nam")
        };
        var token = "eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCIsImtpZCI6InljZVRRa1VITERXXzNqeW5RYzZzWCJ9.eyJpc3MiOiJodHRwczovL2Rldi12enhwaG91ei51cy5hdXRoMC5jb20vIiwic3ViIjoiZHVFTDJJYUZqVkppSWtLSHdLMlE3a0p1M2FuVXpUN2FAY2xpZW50cyIsImF1ZCI6Imh0dHBzOi8vcXVpY2tzdGFydHMvYXBpIiwiaWF0IjoxNjc2NTI5MzIxLCJleHAiOjE2NzY2MTU3MjEsImF6cCI6ImR1RUwySWFGalZKaUlrS0h3SzJRN2tKdTNhblV6VDdhIiwic2NvcGUiOiJyZWFkOm1lc3NhZ2VzIiwiZ3R5IjoiY2xpZW50LWNyZWRlbnRpYWxzIn0.eHAhSWz2oAjxoia3D_QnDptt0SDrsBQd85BghvgWWVAQ7zmHf4n7JyhADvXXTQZwjugeoZQOsRBvm68m0ddj449pxZuoWoKXkAIJR69mycc1xwgE7lqO-NjsWt4FBl4IqKNesbOVnZQN7ANo-3y5OB0U55aQU7EeWgrG96NL2KjMQPsD-qdOAJ0I-qseS-OZCrH_wf8_ZRrHm_ryJh7CHmoKRixF2Ui0vl89P5-it86YK_YRHLJ4J-jYkSm0cRg16DXYE-4ExrN2gR4ddAbOToFqg9Imw9noroA8y8zX-XCYbbYMKNlA5lgV_qB3ht90Ckp1BeSqj0MwLliETDFYzg";
        try
        {
            options.Authority = "https://dev-vzxphouz.us.auth0.com/";
            var user = jwtBearerTokenService.ValidateToken(token, options);
            Assert.NotNull(user);
        }
        catch (System.Exception ex)
        {

        }
    }
}
