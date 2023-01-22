namespace MockServer.Core.Models.Auth;

public class AppAuthorization
{
    public string DisplayName { get; set; }
    public string Policy { get; set; }
    public static AppAuthorization Authorize()
    {
        return new AppAuthorization { DisplayName = nameof(Authorize) };
    }
    public static AppAuthorization AllowAnonymous()
    {
        return new AppAuthorization { DisplayName = nameof(AllowAnonymous) };
    }
}
