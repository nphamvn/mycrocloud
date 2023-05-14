using MockServer.Core.Services;

namespace MockServer.Core.WebApplications;

public class WebApplication : Service
{
    public string WebApplicationId { get; set; }
    public string Description { get; set; }
    public List<string> UseMiddlewares { get; set; }
    public bool Enabled { get; set; }
    public bool Blocked { get; set; }
}