using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Core.Enums;

namespace MockServer.Web.Models.Requests;

public class CreateUpdateRequestViewModel
{
    [Required]
    public RequestType Type { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
    public string Name { get; set; }
    [Required]
    [StringLength(50, ErrorMessage = "Name length can't be more than 8.")]
    public string Path { get; set; }
    public string Method { get; set; }
    public string Description { get; set; }
    public IEnumerable<SelectListItem>? HttpMethods { get; set; }
}
