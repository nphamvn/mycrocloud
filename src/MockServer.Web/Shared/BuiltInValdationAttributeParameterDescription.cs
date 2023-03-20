using Microsoft.AspNetCore.Mvc.Rendering;

namespace MockServer.Web.Shared;

public class BuiltInValdationAttributeParameterDescription
{
    public string Name { get; set; }
    public bool Required { get; set; }
    public string Type { get; set; }
    public IEnumerable<SelectListItem> SelectListItems { get; set; }
}
