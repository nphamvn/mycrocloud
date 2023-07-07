using MockServer.Domain.Services.Entities;

namespace MockServer.Domain.Databases;

public class Database: Service
{
    public string Description { get; set; }
    public string Data { get; set; }
    public string Adapter { get; set; }
    public string JsonFilePath { get; set; }
}
