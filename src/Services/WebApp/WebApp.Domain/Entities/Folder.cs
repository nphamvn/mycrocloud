namespace WebApp.Domain.Entities;

public class Folder : BaseEntity
{
    public int Id { get; set; }
    public int AppId { get; set; }
    public App App { get; set; }
    public string Name { get; set; }
    public int? ParentId { get; set; }
    public Folder Parent { get; set; }
    public ICollection<Folder> Children { get; set; }
    public ICollection<File> Files { get; set; }
}