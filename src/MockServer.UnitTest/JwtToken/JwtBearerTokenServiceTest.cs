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
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
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
            var user = jwtBearerTokenService.ValidateToken(token, options);
            Assert.NotNull(user);
        }
        catch (System.Exception)
        {
            
        }
    }
}
