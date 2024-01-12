namespace NoSql.Core.Entities;

public class Server : BaseEntity
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public string LoginId { get; set; }
    public string Password { get; set; }
    public ICollection<Database> Databases { get; set; }
}