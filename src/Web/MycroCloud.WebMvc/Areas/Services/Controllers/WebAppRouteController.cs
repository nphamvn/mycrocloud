using System.Text;
using System.Text.Json;
using MycroCloud.WebMvc.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MycroCloud.WebMvc.Areas.Services.Models.WebApps;
using MycroCloud.WebMvc.Areas.Services.Services;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MycroCloud.WebMvc.Areas.Services.Controllers;

[Route("[area]/webapp/{WebAppName}/route")]
public class WebAppRouteController(IWebAppRouteService webAppRouteService
    , ILogger<WebAppRouteController> logger) : BaseServiceController
{
    public const string Name = "WebAppRoute";
    private readonly ILogger _logger = logger;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        //base.OnActionExecuting(context);
        ViewData["WebAppName"] = context.ActionArguments["WebAppName"];
    }

    [HttpGet]
    public async Task<IActionResult> Index(int WebAppId, string SearchTerm, string Sort)
    {
        var vm = await webAppRouteService.GetIndexViewModel(WebAppId, SearchTerm, Sort);
        return View("/Areas/Services/Views/WebApp/Route/Index.cshtml", vm);
    }

    [AjaxOnly]
    [HttpGet]
    public async Task<IActionResult> List(int WebAppId, string SearchTerm, string Sort)
    {
        var routes = await webAppRouteService.GetList(WebAppId, SearchTerm, Sort);
        return Ok(routes.Select(r => new
        {
            r.RouteId,
            r.Name,
            r.Description,
            r.MatchPath,
            r.MatchMethods,
            r.MatchOrder
        }));
    }

    [AllowAnonymous]
    [AjaxOnly]
    [HttpGet("/webapps/routes/sample")]
    public IActionResult Sample()
    {
        var json = System.IO.File.ReadAllText("Areas/Services/Views/WebApp/Route/_Partial/sample.json");
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
    public async Task<IActionResult> Get(int WebAppId, int RouteId)
    {
        var route = await webAppRouteService.GetRouteDetails(WebAppId, RouteId);
        return Ok(route);
    }

    [AjaxOnly]
    [HttpPost("Create")]
    public async Task<IActionResult> Create(string WebApplicationName, [FromBody] RouteSaveModel route)
    {
        _logger.LogInformation(JsonSerializer.Serialize(route));
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
        await webAppRouteService.Delete(RouteId);
        return Ok();
    }
}