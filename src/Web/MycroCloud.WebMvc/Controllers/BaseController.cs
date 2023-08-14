using Microsoft.AspNetCore.Mvc;
using MycroCloud.WebMvc.Extentions;
using MycroCloud.WebMvc.Identity;

namespace MycroCloud.WebMvc.Controllers;

[Controller]
public class BaseController : Controller
{
    protected MycroCloudIdentityUser? MycroCloudUser  => User?.ToMycroCloudUser();
}