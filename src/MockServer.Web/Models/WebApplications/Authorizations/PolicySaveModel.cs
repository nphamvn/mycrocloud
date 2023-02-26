using System.ComponentModel.DataAnnotations;

namespace MockServer.Web.Models.WebApplications.Authorizations;

public class PolicySaveModel
{
    [Required]
    public string Name { get; set; }
    public List<string> ConditionalExpressions { get; set; }
    public bool Active { get; set; }
}
