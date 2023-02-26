namespace MockServer.Web.Models.WebApplications.Authorizations;

public class PolicyViewModel
{
    public string Name { get; set; }
    public List<string> ConditionalExpressions { get; set; }
    public bool Active { get; set; }
}
