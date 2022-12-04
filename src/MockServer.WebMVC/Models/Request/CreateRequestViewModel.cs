using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MockServer.Core.Enums;

namespace MockServer.WebMVC.Models.Request;

public class CreateRequestViewModel
{
    public RequestType Type { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
    public string Name { get; set; }
    [Required]
    [StringLength(50, ErrorMessage = "Name length can't be more than 8.")]
    public string Path { get; set; }
    public RequestMethod Method { get; set; }
}
