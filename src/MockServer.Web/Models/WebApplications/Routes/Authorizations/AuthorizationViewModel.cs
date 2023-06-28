using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Core.WebApplication.Authorization;
using MockServer.Web.Models.WebApplications.Authentications;

namespace MockServer.Web.Models.WebApplications.Routes.Authorizations;

public class AuthorizationViewModel
{
    public AuthorizationType Type { get; set; }
    public IEnumerable<SelectListItem>? AuthorizationTypeSelectListItem { get; set; }
    public ICollection<AuthenticationScheme> Type1 { get; set; }
    public IList<int> PolicyIds { get; set; }
    public IEnumerable<SelectListItem>? PolicySelectListItem { get; set; }
    public Dictionary<string, object> Claims { get; set; }
}
