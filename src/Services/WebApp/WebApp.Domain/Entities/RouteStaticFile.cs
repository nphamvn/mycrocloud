namespace WebApp.Domain.Entities;

public class RouteStaticFile : BaseEntity
{
    public int Id { get; set; }
    public int RouteId { get; set; }
    public Route Route { get; set; }
    public string Name { get; set; }
    public byte[] Content { get; set; }
}