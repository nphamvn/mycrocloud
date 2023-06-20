using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockServer.Web.Attributes;
using MockServer.Web.Filters;
using MockServer.Web.Models.Common;
using MockServer.Web.Models.WebApplications.Routes;
using MockServer.Web.Services;
using RouteName = MockServer.Web.Common.Constants.RouteName;
namespace MockServer.Web.Controllers;

[Authorize]
[Route("webapps/{WebApplicationName}/routes")]
[GetAuthUserWebApplicationId(RouteName.WebApplicationName, RouteName.WebApplicationId)]
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

    [AjaxOnly]
    [HttpGet]
    public async Task<IActionResult> List(int WebApplicationId, string SearchTerm, string Sort)
    {
        var routes = await _webApplicationRouteWebService.GetList(WebApplicationId, SearchTerm, Sort);
        return Ok(routes.Select(r => new
        {
            r.RouteId,
            r.Name,
            r.Method,
            r.Path,
            IntegrationType = (int)r.IntegrationType,
            r.Description
        }));
    }
    
    [AjaxOnly]
    [HttpGet("{RouteId:int}")]
    public async Task<IActionResult> Get(int RouteId)
    {
        var route = await _webApplicationRouteWebService.GetDetails(RouteId);
        return Ok(new
        {
            route.RouteId,
            route.Name,
            route.Method,
            route.Path,
            route.Description
        });
    }
    
    [AjaxOnly]
    [HttpPost("create")]
    public async Task<IActionResult> Create(int WebApplicationId, [ModelBinder(BinderType = typeof(RouteModelBinder))] RouteSaveModel route)
    {
        if (!await _webApplicationRouteWebService.ValidateCreate(WebApplicationId, route, ModelState))
        {
            return Ok(new AjaxResult<RouteSaveModel>
            {
                Errors = new List<Error> { new("something went wrong") }
            });
        }
        int id = await _webApplicationRouteWebService.Create(WebApplicationId, route);
        return Ok(id);
    }
    
    [AjaxOnly]
    [HttpPost("edit/{RouteId:int}")]
    public async Task<IActionResult> Edit(int RouteId, [FromBody] RouteSaveModel route)
    {
        if (!await _webApplicationRouteWebService.ValidateEdit(RouteId, route, ModelState))
        {
            var allErrors = ModelState.Values.SelectMany(v => v.Errors);
            return BadRequest(allErrors);
        }
        await _webApplicationRouteWebService.Edit(RouteId, route);
        return NoContent();
    }
    
    [AjaxOnly]
    [HttpPost("delete/{RouteId:int}")]
    public async Task<IActionResult> Delete(int RouteId)
    {
        await _webApplicationRouteWebService.Delete(RouteId);
        return Ok();
    }
}