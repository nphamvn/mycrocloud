using Microsoft.AspNetCore.Mvc;

namespace MockServer.Web.Controllers;

public class AuthorizersController: BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index(int ProjectId)
    {
        return Ok();
    }

    [HttpGet("jwtbearer/{id:int?}")]
    public async Task<IActionResult> ViewJwtBearer(string name, int? id)
    {
        return View();
    }

    [HttpPost("jwtbearer/{id:int?}")]
    public async Task<IActionResult> CreateJwtBearer(string name, int? id)
    {
        return View();
    }

    [HttpPost("jwtbearer/{id:int}/generate-token")]
    public async Task<IActionResult> GenerateJwtBearerToken(string name, int id)
    {
        return View();
    }

    [HttpPost("apikey/create")]
    public async Task<IActionResult> CreateApiKey(string name)
    {
        return View();
    }

    [HttpPost("apikey/{id:int}/generate-key")]
    public async Task<IActionResult> GenerateApiKey(string name, int id)
    {
        return View();
    }

    [HttpGet("apikey/{id:int}")]
    public async Task<IActionResult> ViewApiKey(string name, int id)
    {
        return View();
    }
}
