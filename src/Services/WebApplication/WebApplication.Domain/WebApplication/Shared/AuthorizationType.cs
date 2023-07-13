using WebApplication.Domain.Shared;

namespace WebApplication.Domain.WebApplication.Shared;
public class AuthorizationType : Enumeration
{
    public static AuthorizationType AllowAnonymous = new AuthorizationType(0, "AllowAnonymous");
    public static AuthorizationType Authorized = new AuthorizationType(1, "Authorized");
    protected AuthorizationType(int id, string name) : base(id, name)
    {
    }
}