namespace WebApp.Domain.Entities;

public class AppEntity : BaseEntity
{
    public int AppId { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Enabled { get; set; }
    public bool Blocked { get; set; }
}