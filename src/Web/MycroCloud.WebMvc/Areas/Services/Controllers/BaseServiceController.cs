using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MycroCloud.WebMvc.Controllers;
using MycroCloud.WebMvc.Extentions;
using MycroCloud.WebMvc.Identity;
using MycroCloud.WeMvc;

namespace MycroCloud.WebMvc.Areas.Services.Controllers;

[Area(Constants.Area.Services)]
[Route("[area]/[controller]")]
[Authorize]
public class BaseServiceController : BaseController
{
    
}