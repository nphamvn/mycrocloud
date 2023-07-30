using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MycroCloud.WebMvc.Controllers;

namespace MycroCloud.WebMvc.Areas.Services.Controllers;

[Authorize]
[Route("[area]/[controller]")]
public class BaseServiceController : BaseController
{

}
