namespace MockServer.Core.WebApplications.Security;

public class Policy
{
    public string Name { get; set; }
    public List<string> ConditionalExpressions { get; set; }
}
