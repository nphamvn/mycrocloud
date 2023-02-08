namespace MockServer.Api.Models;

public class IdTokenRequest
{
    public GrantType GrantType { get; set; }
    public string ClientId { get; set; }    //The JWT Id
    public string ClientSecret { get; set; }    //The JWT Secrete
    public string Username { get; set; }
    public string Password { get; set; }
}
public enum GrantType {
    Code,
    ClientCredentials,
    RefreshToken
}
