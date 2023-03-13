using Microsoft.AspNetCore.Mvc.Rendering;

namespace MockServer.Web.Models.Function;

public class FunctionSaveModel
{
    public string Name { get; set; }
    public string Code { get; set; }
    public int RuntimeId { get; set; }
    public IEnumerable<SelectListItem>? RuntimeSelectListItem { get; set; }
}
