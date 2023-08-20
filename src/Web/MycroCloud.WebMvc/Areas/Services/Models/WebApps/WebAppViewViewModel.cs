namespace MycroCloud.WebMvc.Areas.Services.Models.WebApps;

public class WebAppViewViewModel
{
    public int WebAppId { get; set; }
    public string AppName { get; set; }
    public string Description { get; set; }
    public WebAppAccessibility Accessibility { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}