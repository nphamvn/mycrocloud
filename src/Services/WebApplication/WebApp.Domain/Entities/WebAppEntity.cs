namespace WebApp.Domain.Entities;

public class WebAppEntity : BaseEntity
{
    public int WebAppId { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Enabled { get; set; }
    public bool Blocked { get; set; }
}