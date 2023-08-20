
namespace WebApp.Domain.Entities;
public class JwtBearerAuthenticationSchemeEntity : AuthenticationSchemeEntity
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
}