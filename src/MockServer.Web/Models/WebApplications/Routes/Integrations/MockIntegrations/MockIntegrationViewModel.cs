using Microsoft.AspNetCore.Mvc.Rendering;

namespace MockServer.Web.Models.WebApplications.Routes.Integrations.MockIntegrations;

public class MockIntegrationViewModel : RouteIntegrationViewModel
{
    public string Code { get; set; }
    public IList<MockIntegrationResponseHeaderViewModel> ResponseHeaders { get; set; }
    public string ResponseBodyText { get; set; }
    public string ResponseBodyTextFormat { get; set; }
    public int ResponseBodyTextRenderEngine { get; set; }
    public int ResponseStatusCode { get; set; }
    public IEnumerable<SelectListItem> ResponseStatusCodeSelectListItems { get; set; }
    public bool ResponseDelay { get; set; }
    public int ResponseDelayTime { get; set; }
    public string ResponseContentType { get; set; }
    public int ResponseContentLength { get; set; }
}