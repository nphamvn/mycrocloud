using MockServer.Core.WebApplications;

namespace MockServer.Web.Models.WebApplications;

public class WebApplicationIndexItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public WebApplicationAccessibility Accessibility { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string BadgeColor => Accessibility switch
    {
        WebApplicationAccessibility.Public => "bg-light",
        WebApplicationAccessibility.Private => "bg-secondary",
        _ => ""
    };
}