namespace MycroCloud.WebMvc.Areas.Services.Models.WebApps;

public class WebAppModel
{
    public int WebAppId { get; set; }
    public string WebAppName { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}