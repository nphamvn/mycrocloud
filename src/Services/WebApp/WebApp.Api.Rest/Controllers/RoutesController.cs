using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Models;

namespace WebApp.Api.Controllers;

[Route("apps/{appId:int}/routes")]
public class RoutesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Index(int appId, string SearchTerm, string Sort)
    {
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> List(int appId, string SearchTerm, string Sort)
    {
        return Ok();
    }

    [HttpGet("{routeId:int}")]
    public async Task<IActionResult> Get(int routeId)
    {
        return Ok(routeId);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(int appId, RouteCreateUpdateRequest route)
    {
        return Ok(route);
    }
    
    [HttpPost("edit/{routeId:int}")]
    public async Task<IActionResult> Edit(int routeId, RouteCreateUpdateRequest route)
    {
        return NoContent();
    }
    
    [HttpPost("delete/{routeId:int}")]
    public async Task<IActionResult> Delete(int routeId)
    {
        return Ok();
    }
}