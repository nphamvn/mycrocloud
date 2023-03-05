namespace MockServer.Core.WebApplications.Security;

public class Policy: BaseEntity
{
    public int WebApplicationId { get; set; }
    public string Name { get; set; }
    public List<string> ConditionalExpressions { get; set; }
}
