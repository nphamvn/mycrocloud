using System.ComponentModel.DataAnnotations;

namespace MockServer.Web.Models.WebApplications.Routes;

public class RequestHeaderValidationItemSaveModel
{
    [Required]
    public string Name { get; set; }
    public IList<ValidationAttributeSaveModel> Attributes { get; set; }
}
