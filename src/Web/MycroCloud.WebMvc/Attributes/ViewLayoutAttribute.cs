using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MycroCloud.WebMvc.Attributes;

public class ViewLayoutAttribute(string layout) : ResultFilterAttribute
{
    public override void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is ViewResult viewResult)
        {
            viewResult.ViewData["Layout"] = layout;
        }
    }
}
