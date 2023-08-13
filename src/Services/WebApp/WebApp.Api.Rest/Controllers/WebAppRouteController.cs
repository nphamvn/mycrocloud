using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Rest.Models.WebApps.Routes;

namespace MycroCloud.WebApp.Api.Rest.Controllers;

[Authorize]
[Route("webapp/{WebAppName}/route")]
public class WebApplicationRoutesController : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int WebApplicationId, string SearchTerm, string Sort)
    {
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> List(int WebApplicationId, string SearchTerm, string Sort)
    {
        return Ok();
    }

    [HttpGet("sample")]
    public IActionResult Sample()
    {
        var json = System.IO.File.ReadAllText("Views/WebApplications/Routes/_Partial/sample.json");
        var sampleRoute = JsonSerializer.Deserialize<dynamic>(json);
        return Ok(sampleRoute);
    }

    [HttpGet("{RouteId:int}")]
    public async Task<IActionResult> Get(int RouteId)
    {
        var json = System.IO.File.ReadAllText("Views/WebApplications/Routes/_Partial/sample.json");
        var sampleRoute = JsonSerializer.Deserialize<dynamic>(json);
        return Ok(sampleRoute);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(int WebApplicationId, [FromBody] RouteSaveModel route)
    {
        return Ok(route);
    }
    
    [HttpPost("edit/{RouteId:int}")]
    public async Task<IActionResult> Edit(int RouteId, [FromBody] RouteSaveModel route)
    {
        return NoContent();
    }
    
    [HttpPost("delete/{RouteId:int}")]
    public async Task<IActionResult> Delete(int RouteId)
    {
        return Ok();
    }
}