using System.ComponentModel.DataAnnotations;

namespace MockServer.Web.Models.WebApplications.Routes.Integrations.MockIntegrations;

public class MockIntegrationResponseHeaderSaveModel
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Value { get; set; }
}
