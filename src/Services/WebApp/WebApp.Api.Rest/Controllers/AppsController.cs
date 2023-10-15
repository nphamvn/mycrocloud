using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Models;

namespace WebApp.Api.Controllers;

[Route("[controller]")]
public class AppsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery]AppSearchRequest request)
    {
        return Ok(request);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(AppCreateRequest appCreateRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(appCreateRequest);
        }
        
        return Ok(appCreateRequest);
    }

    [HttpGet("{appId:int}/Details")]
    public async Task<IActionResult> Details(int appId)
    {
        return Ok(appId);
    }
    
    [HttpPost("{appId:int/}/Rename")]
    public async Task<IActionResult> Rename(int appId, string newName)
    {
        return RedirectToAction(nameof(Index), new { WebApplicationName = newName });
    }

    [HttpPost("{appId:int}/Delete")]
    public async Task<IActionResult> Delete(int appId)
    {
        return NoContent();
    }
}