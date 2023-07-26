namespace MycroCloud.WebMvc.Areas.Services.Models.WebApp;

public class WebAppIndexItem
{
    public int WebAppId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public WebAppAccessibility Accessibility { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string BadgeColor => Accessibility switch
    {
        WebAppAccessibility.Public => "bg-light",
        WebAppAccessibility.Private => "bg-secondary",
        _ => ""
    };
}