using System.ComponentModel.DataAnnotations;

namespace MockServer.Web.Models.WebApplications.Authentications;

public class AuthenticationSchemeSaveModel
{
    [Required]
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public int Order { get; set; }
    public string Description { get; set; }
    public WebApplication WebApplication { get; set; }
}
