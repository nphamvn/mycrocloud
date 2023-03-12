namespace MockServer.Core.WebApplications.Security;

public class Policy: BaseEntity
{
    public string Name { get; set; }
    public List<string> ConditionalExpressions { get; set; }
    public int WebApplicationId { get; set; }
    public WebApplication WebApplication { get; set; }
}
