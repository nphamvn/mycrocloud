using MockServer.Core.Enums;

namespace MockServer.Core.Models;

public class Project
{
    public int Id { get; set; }
    //public int UserId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ProjectAccessibility Accessibility { get; set; }
    public string PrivateKey { get; set; }
}