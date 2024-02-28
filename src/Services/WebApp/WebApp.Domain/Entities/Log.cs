namespace WebApp.Domain.Entities;

public class Log : BaseEntity
{
    public Guid Id { get; set; }
    public int AppId { get; set; }
    public App App { get; set; }
    public int? RouteId { get; set; }
    public Route Route { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public int StatusCode { get; set; }
    public string AdditionalLogMessage { get; set; }
    public TimeSpan? FunctionExecutionDuration { get; set; }
    public string RemoteIp { get; set; }
}
