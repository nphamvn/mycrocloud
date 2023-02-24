using MockServer.Core.Models;

namespace MockServer.Core.Databases;

public class Database: BaseEntity
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Data { get; set; }
    public string Adapter { get; set; }
    public string JsonFilePath { get; set; }
}
