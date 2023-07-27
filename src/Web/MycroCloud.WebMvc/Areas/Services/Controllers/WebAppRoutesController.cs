using System.Text;
using System.Text.Json;
using MycroCloud.WebMvc.Attributes;
using MycroCloud.WebMvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MycroCloud.WebMvc.Areas.Services.Models.WebApps;
using MycroCloud.WebMvc.Areas.Services.Services;
using MycroCloud.WeMvc;

namespace MycroCloud.WebMvc.Areas.Services.Controllers;

[Authorize]
[Route("webapps/{WebApplicationName}/routes")]
[GetAuthUserWebApplicationId(Constants.RouteName.WebAppName, Constants.RouteName.WebAppId)]
public class WebAppRoutesController : Controller
{
    private readonly IWebAppRouteService _webAppRouteService;

    public WebAppRoutesController(IWebAppRouteService webAppRouteService)
    {
        _webAppRouteService = webAppRouteService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(int WebApplicationId, string SearchTerm, string Sort)
    {
        var vm = await _webAppRouteService.GetIndexViewModel(WebApplicationId, SearchTerm, Sort);
        return View("/Areas/Services/Views/WebApp/Route/Index.cshtml", vm);
    }

    [AjaxOnly]
    [HttpGet]
    public async Task<IActionResult> List(int WebApplicationId, string SearchTerm, string Sort)
    {
        var routes = await _webAppRouteService.GetList(WebApplicationId, SearchTerm, Sort);
        return Ok(routes.Select(r => new
        {
            r.RouteId,
            r.Name,
            r.Description
        }));
    }

    [AjaxOnly]
    [HttpGet("sample")]
    public IActionResult Sample()
    {
        var json = System.IO.File.ReadAllText("Views/WebApplications/Routes/_Partial/sample.json");
        var sampleRoute = JsonSerializer.Deserialize<dynamic>(json);
        return Ok(sampleRoute);
    }

    //[AjaxOnly]
    [HttpGet("downloadJson")]
    public IActionResult DownloadJson()
    {
        var json = System.IO.File.ReadAllText("Views/WebApplications/Routes/_Partial/sample.json");
        var byteArray = Encoding.ASCII.GetBytes(json);
        return File(byteArray, "application/json", "sample.json");
    }

    [AjaxOnly]
    [HttpGet("{RouteId:int}")]
    public async Task<IActionResult> Get(int RouteId)
    {
        var json = System.IO.File.ReadAllText("Views/WebApplications/Routes/_Partial/sample.json");
        var sampleRoute = JsonSerializer.Deserialize<dynamic>(json);
        return Ok(sampleRoute);
        //return ok(route);
    }

    [AjaxOnly]
    [HttpPost("create")]
    public async Task<IActionResult> Create(int WebApplicationId, [FromBody] RouteSaveModel route)
    {
        return Ok(route);
    }
    
    [AjaxOnly]
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
    
    [AjaxOnly]
    [HttpPost("delete/{RouteId:int}")]
    public async Task<IActionResult> Delete(int RouteId)
    {
        await _webAppRouteService.Delete(RouteId);
        return Ok();
    }
}