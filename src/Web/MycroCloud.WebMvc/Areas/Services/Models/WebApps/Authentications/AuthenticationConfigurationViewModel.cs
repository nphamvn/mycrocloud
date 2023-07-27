
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MycroCloud.WebMvc.Areas.Services.Models.WebApps;

public class AuthenticationConfigurationViewModel
{
    public WebAppViewViewModel WebAppViewViewModel { get; set; }
    public IEnumerable<SelectListItem> AuthenticationSchemeSelectListItem { get; set; }
    public IEnumerable<int> SelectedAuthenticationSchemeIds { get; set; }
}