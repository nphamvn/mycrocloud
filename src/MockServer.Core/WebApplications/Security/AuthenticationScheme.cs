namespace MockServer.Core.WebApplications.Security;

public class AuthenticationScheme : BaseEntity
{
    public int SchemeId { get; set; }
    public int WebApplicationId { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public AuthenticationSchemeType Type { get; set; }
    public int Order { get; set; }
    public AuthenticationSchemeOptions Options { get; set; }
    public string Description { get; set; }
}
