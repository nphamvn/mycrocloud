using WebApp.Domain.Enums;

namespace WebApp.Domain.Entities;

public class App : BaseEntity
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public AppStatus Status { get; set; }
    public ICollection<Route> Routes { get; set; }
    public ICollection<Log> Logs { get; set; }
    public AppSettings Settings { get; set; }
    public ICollection<Variable> Variables { get; set; }
    public CorsSettings CorsSettings { get; set; }
}