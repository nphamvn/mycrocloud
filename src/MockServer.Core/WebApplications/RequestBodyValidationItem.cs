
namespace MockServer.Core.WebApplications;

public class RequestBodyValidationItem
{
    public string Field { get; set; }
    public IList<ValidationAttribute> Attributes { get; set; }
}
