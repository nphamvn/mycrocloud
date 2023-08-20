using System.ComponentModel.DataAnnotations;

namespace MycroCloud.WebMvc.Areas.Services.Models.WebApps;

public class JwtBearerSchemeSaveViewModel
{
    [Display(Name= "Name")]
    [Required]
    public string Name { get; set; }

    [Display(Name = "Display Name")]
    public string DisplayName { get; set; }

    [Display(Name = "Description")]
    public string Description { get; set; }
    [Required]
    public string Authority { get; set; }
    public string Audience { get; set; }
}