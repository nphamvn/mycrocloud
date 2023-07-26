namespace MicroCloud.Web.Models.WebApplications.Authorizations;

public class PolicyIndexViewModel
{
    public WebAppModel WebAppModel { get; set; }
    public IEnumerable<PolicyIndexItem> Policies { get; set; }
}
