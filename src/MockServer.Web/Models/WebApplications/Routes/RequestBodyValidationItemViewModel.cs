
namespace MockServer.Web.Models.WebApplications.Routes;

public class RequestBodyValidationItemViewModel
{
    public string Field { get; set; }
    public IList<ValidationAttributeViewModel> Attributes { get; set; }
}
