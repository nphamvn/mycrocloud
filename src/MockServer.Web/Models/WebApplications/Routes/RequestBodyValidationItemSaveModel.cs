using System.ComponentModel.DataAnnotations;

namespace MockServer.Web.Models.WebApplications.Routes;

public class RequestBodyValidationItemSaveModel
{
    [Required]
    public string Field { get; set; }
    public IList<ValidationAttributeSaveModel> Attributes { get; set; }
}
