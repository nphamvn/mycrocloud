using System.ComponentModel.DataAnnotations;
using MockServer.Core.Enums;

namespace MockServer.Web.Models.Project;

public class CreateProjectViewModel
{
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Please enter the project name.")]
    public string Name { get; set; }
    public ProjectAccessibility Accessibility { get; set; }
    public string Description { get; set; }
}