using MycroCloud.Shared;

namespace WebApp.Domain.Shared;
public class RouteAuthorizationType : Enumeration
{
    public static RouteAuthorizationType AllowAnonymous = new (0, "AllowAnonymous");
    public static RouteAuthorizationType Authorized = new (1, "Authorized");
    protected RouteAuthorizationType(int id, string name) : base(id, name)
    {
    }
}