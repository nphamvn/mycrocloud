namespace WebApp.Domain.Entities;

public class CorsSettings
{
    public List<string> AllowedHeaders { get; set; }
    public List<string> AllowedMethods { get; set; }
    public List<string> AllowedOrigins { get; set; }
    public List<string> ExposeHeaders { get; set; }
    public int? MaxAgeSeconds { get; set; }
}