
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MicroCloud.Web.Models.WebApplications.Authentications;

public class AuthenticationSettingsModel
{
    public AuthenticationSettingsModel()
    {
        SelectedAuthenticationSchemeIds = new List<int>();
    }
    public WebApplication WebApplication { get; set; }
    public IEnumerable<SelectListItem> AuthenticationSchemeSelectListItem { get; set; }
    public IEnumerable<int> SelectedAuthenticationSchemeIds { get; set; }
}