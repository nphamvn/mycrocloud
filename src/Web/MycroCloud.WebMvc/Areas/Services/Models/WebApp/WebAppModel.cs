namespace MycroCloud.WebMvc.Areas.Services.Models.WebApp;

public class WebAppModel
{
    public int WebAppId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public WebAppAccessibility Accessibility { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}