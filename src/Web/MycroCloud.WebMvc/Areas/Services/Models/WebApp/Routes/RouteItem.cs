namespace MycroCloud.WebMvc.Areas.Services.Models.WebApp;

public class RouteItem
{
    public int RouteId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public RouteMatchSaveModel Match { get; set; }
}