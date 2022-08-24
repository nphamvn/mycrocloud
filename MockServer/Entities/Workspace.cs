using System.Collections.Generic;

namespace MockServer.Entities;

public class Workspace
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string FriendlyName { get; set; }
    public int AccessScope { get; set; } = 1;//0: private, 1: public
    public string? ApiKey { get; set; }
    public ICollection<Request> Requests { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}