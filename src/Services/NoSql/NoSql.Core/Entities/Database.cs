
namespace NoSql.Core.Entities;

public class Database : BaseEntity
{
    public int Id { get; set; }
    public Server Server { get; set; }
    public int ServerId { get; set; }
    public string Name { get; set; }
    //TODO:
    public object Schema { get; set; } = new();
    public object Data { get; set; } = new();
}