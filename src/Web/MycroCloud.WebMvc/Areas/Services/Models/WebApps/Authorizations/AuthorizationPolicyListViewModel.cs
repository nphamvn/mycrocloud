namespace MycroCloud.WebMvc.Areas.Services.Models.WebApps;

public class AuthorizationPolicyListViewModel
{
    public WebAppModel WebApp { get; set; }
    public IEnumerable<PolicyListViewItem> Policies { get; set; }
}
public class PolicyListViewItem
{
    public int Id { get; set; } 
    public int PolicyId { get; set; } 
    public string Name { get; set; }
}