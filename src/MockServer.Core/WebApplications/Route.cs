using MockServer.Core.WebApplications.Security;

namespace MockServer.Core.WebApplications;
public class Route : BaseEntity
{
    public int WebApplicationId { get; set; }
    public RouteIntegrationType IntegrationType { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Description { get; set; }
    public string Method { get; set; }
    public Authorization Authorization { get; set; }
    public IList<RequestQueryValidationItem> RequestQueryValidationItems { get; set; }
    public IList<RequestHeaderValidationItem> RequestHeaderValidationItems { get; set; }
    public IList<RequestBodyValidationItem> RequestBodyValidationItems { get; set; }
}

