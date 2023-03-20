namespace MockServer.Web.Models.WebApplications.Routes;

public class RouteRequestBody
{
    public List<string> Constraints { get; set; }
    /// <summary>
    /// key: field name
    /// value: key: attribute name, value: parameter
    /// </summary>
    public Dictionary<string, Dictionary<string, string>> FieldValidationAttributes { get; set; }
    public string Text { get; set; }
}
