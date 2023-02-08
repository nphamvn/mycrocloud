using Microsoft.AspNetCore.Mvc;

namespace MockServer.Web.Controllers;

public class UserPoolsController: BaseController
{
    public async Task<IActionResult> Index() {
        return Ok();
    }
    [HttpGet("new")]
    public async Task<IActionResult> New()
    {
        return Ok();
    }
    [HttpPost("new")]
    public async Task<IActionResult> New(string name)
    {
        return Ok();
    }
}
