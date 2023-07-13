using System.ComponentModel.DataAnnotations;

namespace MicroCloud.Web.Models.WebApplications;

public class WebApplicationCreateModel
{
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Please enter the application name.")]
    public string Name { get; set; }
    public string Description { get; set; }
}