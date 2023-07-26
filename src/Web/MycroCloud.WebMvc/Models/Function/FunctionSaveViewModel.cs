using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MicroCloud.Web.Models.Function;

public class FunctionSaveViewModel
{
    [Required]
    public string Name { get; set; }
    [Required]
    public int RuntimeId { get; set; }
    public string Code { get; set; }
    public IEnumerable<SelectListItem>? RuntimeSelectListItem { get; set; }
}
