using WebApplication.Domain.Services.Entities;

namespace WebApplication.Domain.Databases;

public class Database: Service
{
    public string Description { get; set; }
    public string Data { get; set; }
    public string Adapter { get; set; }
    public string JsonFilePath { get; set; }
}
