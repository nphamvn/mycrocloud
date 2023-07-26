using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MicroCloud.Web.Models.WebApplications.Authentications;

public class AuthenticationSchemeViewModel
{
    [DisplayName("Name")]
    [Required]
    public string Name { get; set; }

    [DisplayName("Display Name")]
    public string DisplayName { get; set; }

    [DisplayName("Description")]
    public string Description { get; set; }

    public WebAppModel WebAppModel { get; set; }
}
