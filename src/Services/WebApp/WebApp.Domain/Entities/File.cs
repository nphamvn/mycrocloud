namespace WebApp.Domain.Entities;

public class File : BaseEntity
{
    public int Id { get; set; }
    public int FolderId { get; set; }
    public Folder Folder { get; set; }
    public string Name { get; set; }
    public byte[] Content { get; set; }
}