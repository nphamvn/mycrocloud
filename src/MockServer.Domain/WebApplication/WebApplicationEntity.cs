using MockServer.Domain.Services.Entities;

namespace MockServer.Domain.WebApplication;

public class WebApplicationEntity : Service
{
    public int WebApplicationId { get; set; }
    public string Description { get; set; }
    public bool Enabled { get; set; }
    public bool Blocked { get; set; }
}