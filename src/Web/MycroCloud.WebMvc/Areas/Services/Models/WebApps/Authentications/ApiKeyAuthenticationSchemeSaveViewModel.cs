using System.ComponentModel.DataAnnotations;

namespace MycroCloud.WebMvc.Areas.Services.Models.WebApps;

public class ApiKeyAuthenticationSchemeSaveViewModel
{
    [Display(Name= "Name")]
    [Required]
    public string Name { get; set; }

    [Display(Name = "Display Name")]
    public string DisplayName { get; set; }

    [Display(Name = "Description")]
    public string Description { get; set; }
    public ApiKeySchemeOptionsSaveModel Options { get; set; }
}
public class ApiKeySchemeOptionsSaveModel
{
    
}