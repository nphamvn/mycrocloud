using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MockServer.Web.Models.Function;

public class FunctionSaveModel
{
    [Required]
    public string Name { get; set; }
    [Required]
    public int RuntimeId { get; set; }
    public string Code { get; set; }
    public IEnumerable<SelectListItem>? RuntimeSelectListItem { get; set; }
}
