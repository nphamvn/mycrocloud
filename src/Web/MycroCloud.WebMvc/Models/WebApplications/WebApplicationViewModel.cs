namespace MicroCloud.Web.Models.WebApplications;

public class WebApplicationViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public IEnumerable<Route> Routes { get; set; }
}