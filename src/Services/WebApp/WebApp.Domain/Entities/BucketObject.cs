namespace WebApp.Domain.Entities;

public class BucketObject : BaseEntity
{
    public App App { get; set; }
    public int AppId { get; set; }
    public string Key { get; set; }
    public byte[] Content { get; set; }
}