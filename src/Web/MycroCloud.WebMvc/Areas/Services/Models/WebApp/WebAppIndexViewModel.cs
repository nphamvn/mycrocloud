namespace MycroCloud.WebMvc.Areas.Services.Models.WebApp;
public class WebAppIndexViewModel
{
    public WebAppSearchModel Search { get; set; } = new WebAppSearchModel();
    public IEnumerable<WebAppIndexItem> Applications { get; set; }
}