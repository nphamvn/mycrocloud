namespace MycroCloud.WebMvc.Areas.Services.Models.WebApps;

public class AuthenticationSchemeListViewModel
{
    public WebAppModel WebApp { get; set; }
    public IEnumerable<AuthenticationSchemeIndexItem> AuthenticationSchemes { get; set; }
    public IEnumerable<int> SelectedAuthenticationSchemeIds { get; set; }
}
public class AuthenticationSchemeIndexItem
{
    public int Id { get; set; }
    public AuthenticationSchemeType Type { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public int Order { get; set; }
    public string Description { get; set; }
}