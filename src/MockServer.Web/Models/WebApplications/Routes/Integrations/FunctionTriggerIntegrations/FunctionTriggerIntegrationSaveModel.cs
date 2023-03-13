using Microsoft.AspNetCore.Mvc.Rendering;

namespace MockServer.Web.Models.WebApplications.Routes.Integrations.FunctionTriggerIntegrations;

public class FunctionTriggerIntegrationSaveModel
{
    public int FunctionId { get; set; }
    public IEnumerable<SelectListItem> FunctionSelectListItem { get; set; }
}
