namespace MockServer.Web.Models;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int AccessScope { get; set; }
    public string Key { get; set; }
}