namespace WebApp.Domain.Entities;
public class AuthorizationPolicyEntity
{
    public int PolicyId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Claims Claims { get; set; }
    public List<string> ConditionalExpressions { get; set; }
    public int WebAppId { get; set; }
    public WebAppEntity WebAppEntity { get; set; }
}

public class Claims : Dictionary<string, string>
{

}