using WebApp.Domain.Enums;

namespace WebApp.Domain.Entities;

public class Route : BaseEntity
{
    public int Id { get; set; }
    public int AppId { get; set; }
    public App App { get; set; }
    public string Name { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public string Description { get; set; }
    public string ResponseType { get; set; }
    public int? ResponseStatusCode { get; set; }
    public IList<ResponseHeader> ResponseHeaders { get; set; }
    public string ResponseBody { get; set; }
    public string ResponseBodyLanguage { get; set; }
    public string FunctionHandler { get; set; }
    public IList<string> FunctionHandlerDependencies { get; set; }
    public IList<RouteValidation> Validations { get; set; }
    public bool RequireAuthorization { get; set; }
    public RouteStatus Status { get; set; } = RouteStatus.Active;
}

public class RouteValidation
{
    public int Id { get; set; }
    public int RouteId { get; set; }
    public Route Route { get; set; }
    public string Source { get; set; }
    public string Name { get; set; }

    /// <summary>
    /// Key: Rule name e.g. required, Value: Rule details
    /// </summary>
    public Dictionary<string, object> Rules { get; set; }

    public List<string> Expressions { get; set; }
}

public class ResponseHeader
{
    public string Name { get; set; }
    public string Value { get; set; }
}