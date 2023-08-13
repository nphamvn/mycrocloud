using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Rest.Models.WebApps;

namespace MycroCloud.WebApp.Api.Rest.Controllers;

[Authorize]
[Route("webapp")]
public class WebApplicationsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> List(ListWebAppRequest request)
    {
        return Ok(request);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(CreateWebAppRequest createWebAppRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(createWebAppRequest);
        }

        return Ok(createWebAppRequest);
    }

    [HttpGet("{WebAppName}/Details")]
    public async Task<IActionResult> Details(int WebApplicationId)
    {
        return Ok(WebApplicationId);
    }
    
    [HttpPost("Rename")]
    public async Task<IActionResult> Rename(int WebApplicationId, string newName)
    {
        return RedirectToAction(nameof(List), new { WebApplicationName = newName });
    }

    [HttpPost("Delete")]
    public async Task<IActionResult> Delete(int WebApplicationId)
    {
        return NoContent();
    }
}