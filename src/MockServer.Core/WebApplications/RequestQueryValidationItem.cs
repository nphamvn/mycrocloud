
namespace MockServer.Core.WebApplications;

public class RequestQueryValidationItem
{
    public string Key { get; set; }
    public IList<ValidationAttribute> Attributes { get; set; }
}
