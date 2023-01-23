namespace MockServer.Core.Models.Auth;

public class AppAuthorization
{
    public string Title { get; set; }
    public string Policy { get; set; }
    public static AppAuthorization Authorize()
    {
        return new AppAuthorization { Title = nameof(Authorize) };
    }
    public static AppAuthorization AllowAnonymous()
    {
        return new AppAuthorization { Title = nameof(AllowAnonymous) };
    }
}
