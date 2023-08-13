using Microsoft.AspNetCore.Mvc;
using MycroCloud.WebMvc.Extentions;
using MycroCloud.WebMvc.Identity;

namespace MycroCloud.WebMvc.Controllers;

[Controller]
public class BaseController : Controller
{
    public const int ControllerLength = 10;
    protected MycroCloudIdentityUser AuthenticatedMycroCloudUser
        => User.ToMycroCloudUser();
}