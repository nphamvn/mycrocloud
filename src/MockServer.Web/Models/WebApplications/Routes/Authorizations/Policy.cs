namespace MockServer.Web.Models.WebApplications.Routes.Authorizations;

public class AuthorizationPolicy
{
    public string Name { get; set; }
    public List<string> ConditionalExpressions { get; set; }
    public bool Active { get; set; }
}
