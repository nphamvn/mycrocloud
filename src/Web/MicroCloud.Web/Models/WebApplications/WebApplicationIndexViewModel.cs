namespace MicroCloud.Web.Models.WebApplications;
public class WebApplicationIndexViewModel
{
    public WebApplicationSearchModel Search { get; set; } = new WebApplicationSearchModel();
    public IEnumerable<WebApplicationIndexItem> Applications { get; set; }
}