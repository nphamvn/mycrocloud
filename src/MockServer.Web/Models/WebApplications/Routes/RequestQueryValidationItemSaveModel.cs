using System.ComponentModel.DataAnnotations;

namespace MockServer.Web.Models.WebApplications.Routes;

public class RequestQueryValidationItemSaveModel
{
    [Required]
    public string Key { get; set; }
    public IList<ValidationAttributeSaveModel> Attributes { get; set; }
}
