using System.ComponentModel.DataAnnotations;

namespace MycroCloud.WebMvc.Areas.Services.Models.WebApps;

public class WebAppCreateViewModel
{
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Please enter the application name.")]
    public string Name { get; set; }
    public string Description { get; set; }
}