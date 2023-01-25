namespace MockServer.Core.Models.Auth;

public class AppAuthorization
{
    public string Title { get; set; }
    public string AuthenticationSchemes  { get; set; }
    public string Policy { get; set; }
    public static AppAuthorization Authorize()
    {
        return new AppAuthorization { Title = nameof(Authorize) };
    }
    public static AppAuthorization AllowAnonymous()
    {
        return new AppAuthorization { Title = nameof(AllowAnonymous) };
    }
    public IList<Requirement> Requirements { get; set; }
}

public class Requirement {
    public string Name { get; set; }
    public string ConditionalExpression { get; set; }
}
