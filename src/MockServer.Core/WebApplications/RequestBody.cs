namespace MockServer.Core.WebApplications;

public class RouteRequestBody
{
    public string Text { get; set; }
    public string TextFormat { get; set; }
    public List<RequestBodyFieldConstraint> FieldConstraints { get; set; }
    public List<string> Constraints { get; set; }
}