namespace MockServer.Domain.WebApplication.Route;
public class RouteValidation
{
    public ICollection<QueryParameterValidationItem> QueryParameters { get; set; }
    public ICollection<HeaderValidationItem> Headers { get; set; }
    public ICollection<BodyValidationItem> Body { get; set; }
}