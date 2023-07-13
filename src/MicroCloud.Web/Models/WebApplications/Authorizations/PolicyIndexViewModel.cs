namespace MicroCloud.Web.Models.WebApplications.Authorizations;

public class PolicyIndexViewModel
{
    public WebApplication WebApplication { get; set; }
    public IEnumerable<PolicyIndexItem> Policies { get; set; }
}
