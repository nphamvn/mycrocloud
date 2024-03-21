namespace WebApp.Domain.Entities;

public class TextStorage : BaseEntity
{
    public int Id { get; set; }
    public int AppId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Content { get; set; }
    public App App { get; set; }
}