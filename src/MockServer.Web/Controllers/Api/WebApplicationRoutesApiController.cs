using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MockServer.Core.WebApplications;
using MockServer.Web.Filters;
using MockServer.Web.Models.Common;
using MockServer.Web.Models.WebApplications.Routes;
using MockServer.Web.Models.WebApplications.Routes.Integrations.MockIntegrations;
using MockServer.Web.Services;
using RouteName = MockServer.Web.Common.Constants.RouteName;
namespace MockServer.Web.Controllers.Api;

[Authorize]
[Route("api/webapps/{WebApplicationName}/routes")]
[GetAuthUserWebApplicationId(RouteName.WebApplicationName, RouteName.WebApplicationId)]
public class WebApplicationRoutesApiController : ApiController
{
    private readonly IWebApplicationRouteWebService _webApplicationRouteWebService;
    private readonly IWebApplicationWebService _webApplicationWebService;

    public WebApplicationRoutesApiController(
        IWebApplicationRouteWebService webApplicationRouteWebService,
        IWebApplicationWebService webApplicationWebService)
    {
        _webApplicationRouteWebService = webApplicationRouteWebService;
        _webApplicationWebService = webApplicationWebService;
    }

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

    [HttpGet("{RouteId:int}")]
    [ValidateRouteWebApplication(RouteName.WebApplicationId, RouteName.RouteId)]
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

    [HttpPost("create")]
    public async Task<IActionResult> Create(int WebApplicationId, [FromBody] RouteSaveModel route)
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

    [HttpPost("edit/{RouteId:int}")]
    [ValidateRouteWebApplication(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> Edit(int RouteId, [FromBody] RouteSaveModel route)
    {
        if (!await _webApplicationRouteWebService.ValidateEdit(RouteId, route, ModelState))
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            return BadRequest(allErrors);
        }
        await _webApplicationRouteWebService.Edit(RouteId, route);
        return NoContent();
    }

    [HttpPost("delete")]
    [ValidateRouteWebApplication(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> Delete(int RouteId)
    {
        await _webApplicationRouteWebService.Delete(RouteId);
        return Ok();
    }

    [HttpGet("{RouteId:int}/mock-integration")]
    [ValidateRouteWebApplication(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> MockIntegration(int RouteId)
    {
        var integration = await _webApplicationRouteWebService.GetMockIntegration(RouteId);
        return Ok(new AjaxResult<MockIntegrationViewModel>
        {
            Data = integration
        });
    }

    [HttpPost("{RouteId:int}/mock-integration")]
    [ValidateRouteWebApplication(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> MockIntegration(int RouteId, MockIntegrationSaveModel integration)
    {
        await _webApplicationRouteWebService.SaveMockIntegration(RouteId, integration);
        return Ok(new AjaxResult<MockIntegrationSaveModel>
        {
            Data = integration
        });
    }
}