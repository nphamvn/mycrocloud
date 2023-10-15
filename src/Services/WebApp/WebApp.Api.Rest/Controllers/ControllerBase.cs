using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ControllerBase : Microsoft.AspNetCore.Mvc.ControllerBase
{
    
}