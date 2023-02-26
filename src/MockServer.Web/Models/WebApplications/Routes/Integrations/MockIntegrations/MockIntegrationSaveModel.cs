using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MockServer.Web.Models.WebApplications.Routes.Integrations.MockIntegrations;

public class MockIntegrationSaveModel : RouteIntegrationSaveModel
{
    public string Code { get; set; }
    public IList<MockIntegrationResponseHeaderSaveModel> ResponseHeaders { get; set; }
    [Required]
    public string ResponseBodyText { get; set; }
    public string ResponseBodyTextFormat { get; set; }
    public int ResponseBodyTextRenderEngine { get; set; }
    [Required]
    public int ResponseStatusCode { get; set; }
    public IEnumerable<SelectListItem> ResponseStatusCodeSelectListItems { get; set; }
    [Required]
    public bool ResponseDelay { get; set; }
    public int ResponseDelayTime { get; set; }
    public string ResponseContentType { get; set; }
    public int ResponseContentLength { get; set; }
}