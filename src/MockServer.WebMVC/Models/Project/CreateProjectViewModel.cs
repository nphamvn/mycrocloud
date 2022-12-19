using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MockServer.Core.Enums;

namespace MockServer.WebMVC.Models.Project;

public class CreateProjectViewModel
{
    [Display(Name = "Name")]
    [Required(ErrorMessage = "Please enter the project name.")]
    //[RegularExpression(@"^[a-zA-Z''-']{1,40}$", ErrorMessage = "Characters are not allowed.")]
    public string Name { get; set; }
    public ProjectAccessibility Accessibility { get; set; }
    public string Description { get; set; }
}
