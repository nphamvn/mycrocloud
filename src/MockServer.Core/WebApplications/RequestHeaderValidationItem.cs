
namespace MockServer.Core.WebApplications;

public class RequestHeaderValidationItem
{
    public string Name { get; set; }
    public IList<ValidationAttribute> Attributes { get; set; }
}
