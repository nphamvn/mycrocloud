using WebApplication.Domain.Services.Entities;

namespace WebApplication.Domain.WebApplication.Entities;

public class WebApplication : Service
{
    public int WebApplicationId { get; set; }
    public string Description { get; set; }
    public bool Enabled { get; set; }
    public bool Blocked { get; set; }
}