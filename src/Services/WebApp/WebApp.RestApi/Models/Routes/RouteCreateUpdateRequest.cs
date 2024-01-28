using Route = WebApp.Domain.Entities.Route;
namespace WebApp.RestApi.Models.Routes;

public class RouteCreateUpdateRequest
{
    public string Name { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public string ResponseType { get; set; }
    public int? ResponseStatusCode { get; set; }
    public List<ResponseHeader> ResponseHeaders { get; set; } = [];
    public string? ResponseBody { get; set; }
    public bool UseDynamicResponse { get; set; }
    public string? ResponseBodyLanguage { get; set; }
    public string? FunctionHandler { get; set; }
    public List<string>? FunctionHandlerDependencies { get; set; }
    public bool RequireAuthorization { get; set; }
    public Route ToEntity()
    {
        return new Route
        {
            Name = Name,
            Method = Method,
            Path = Path,
            ResponseType = ResponseType,
            ResponseStatusCode = ResponseStatusCode,
            ResponseHeaders = ResponseHeaders.Select(h => h.ToEntity()).ToList(),
            ResponseBodyLanguage = ResponseBodyLanguage,
            ResponseBody = ResponseBody,
            UseDynamicResponse = UseDynamicResponse,
            FunctionHandler = FunctionHandler,
            FunctionHandlerDependencies = FunctionHandlerDependencies,
            RequireAuthorization = RequireAuthorization
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
        route.UseDynamicResponse = UseDynamicResponse;
        route.FunctionHandler = FunctionHandler;
        route.FunctionHandlerDependencies = FunctionHandlerDependencies;
        route.RequireAuthorization = RequireAuthorization;
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