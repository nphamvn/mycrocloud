namespace MockServer.Core.Enums;

public enum AuthenticationSchemeType
{
    JwtBearer = 1,
    ApiKey = 2,
    Basic = 3
}
public enum AuthorizationType
{
    None,
    AllowAnonymous,
    Authorize
}
