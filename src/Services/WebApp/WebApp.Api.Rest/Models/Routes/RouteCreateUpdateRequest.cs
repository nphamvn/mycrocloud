using Route = WebApp.Domain.Entities.Route;
namespace WebApp.Api.Models;

public class RouteCreateUpdateRequest
{
    public string Name { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public string ResponseType { get; set; }
    public int? ResponseStatusCode { get; set; }
    public List<ResponseHeader> ResponseHeaders { get; set; } = [];
    public string? ResponseBody { get; set; }
    public string? ResponseBodyLanguage { get; set; }
    public string? FunctionHandler { get; set; }

    public Route ToEntity()
    {
        return new Route() {
            Name = Name,
            Method = Method,
            Path = Path,
            ResponseType = ResponseType,
            ResponseStatusCode = ResponseStatusCode,
            ResponseHeaders = ResponseHeaders.Select(h => h.ToEntity()).ToList(),
            ResponseBodyLanguage = ResponseBodyLanguage,
            ResponseBody = ResponseBody,
            FunctionHandler = FunctionHandler,
        };
    }
    public void ToEntity(Route route)
    {
        route.Name = Name;
        route.Method = Method;
        route.Path = Path;
        route.ResponseType = ResponseType;
        route.ResponseStatusCode = ResponseStatusCode;
        route.ResponseHeaders = ResponseHeaders?.Select(h => h.ToEntity()).ToList();
        route.ResponseBodyLanguage = ResponseBodyLanguage;
        route.ResponseBody = ResponseBody;
        route.FunctionHandler = FunctionHandler;
    }
}

public class ResponseHeader
{
    public string Name { get; set; }
    public string Value { get; set; }

    public Domain.Entities.ResponseHeader ToEntity()
    {
        return new Domain.Entities.ResponseHeader() {
            Name = Name,
            Value = Value
        };
    }
}