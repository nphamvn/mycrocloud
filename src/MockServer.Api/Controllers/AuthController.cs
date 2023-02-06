using Microsoft.AspNetCore.Mvc;

namespace MockServer.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    [Route("auth")]
    public async Task<IActionResult> Auth()
    {
        return Ok();
    }
}