namespace WebApp.Domain.Entities;

public class Database : BaseEntity
{
    public int Id { get; set; }
    public int ServerId { get; set; }
    public string Name { get; set; }
    public object Schema { get; set; }
    public object Data { get; set; }
    public Server Server { get; set; }
}