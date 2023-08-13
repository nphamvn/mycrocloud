using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MycroCloud.WebApp.Api.Rest.Controllers;

[Authorize]
[Route("webapp/{WebApplicationName}/routes")]
public class WebApplicationRoutesController : Controller
{
    private readonly IWebApplicationRouteService _webApplicationRouteWebService;

    public WebApplicationRoutesController(IWebApplicationRouteService webApplicationRouteWebService)
    {
        _webApplicationRouteWebService = webApplicationRouteWebService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(int WebApplicationId, string SearchTerm, string Sort)
    {
        var pm = await _webApplicationRouteWebService.GetIndexViewModel(WebApplicationId, SearchTerm, Sort);
        return View("/Views/WebApplications/Routes/Index.cshtml", pm);
    }

    [HttpGet]
    public async Task<IActionResult> List(int WebApplicationId, string SearchTerm, string Sort)
    {
        var routes = await _webApplicationRouteWebService.GetList(WebApplicationId, SearchTerm, Sort);
        return Ok(routes.Select(r => new
        {
            r.RouteId,
            r.Name,
            r.Description
        }));
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
        //return ok(route);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(int WebApplicationId, [FromBody] RouteSaveModel route)
    {
        return Ok(route);
    }
    
    [HttpPost("edit/{RouteId:int}")]
    public async Task<IActionResult> Edit(int RouteId, [FromBody] RouteSaveModel route)
    {
        // if (!await _webApplicationRouteWebService.ValidateEdit(RouteId, route, ModelState))
        // {
        //     var allErrors = ModelState.Values.SelectMany(v => v.Errors);
        //     return BadRequest(allErrors);
        // }
        // await _webApplicationRouteWebService.Edit(RouteId, route);
        return NoContent();
    }
    
    [HttpPost("delete/{RouteId:int}")]
    public async Task<IActionResult> Delete(int RouteId)
    {
        await _webApplicationRouteWebService.Delete(RouteId);
        return Ok();
    }
}

public interface IWebApplicationRouteService
{
}