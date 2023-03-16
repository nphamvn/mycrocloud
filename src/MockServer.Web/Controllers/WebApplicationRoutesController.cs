using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MockServer.Core.WebApplications;
using MockServer.Web.Attributes;
using MockServer.Web.Filters;
using MockServer.Web.Models.Common;
using MockServer.Web.Models.WebApplications.Routes;
using MockServer.Web.Models.WebApplications.Routes.Integrations.MockIntegrations;
using MockServer.Web.Services;
using RouteName = MockServer.Web.Common.Constants.RouteName;
namespace MockServer.Web.Controllers;

[Authorize]
[Route("webapps/{WebApplicationName}/routes")]
[GetAuthUserWebApplicationId(RouteName.WebApplicationName, RouteName.WebApplicationId)]
public class WebApplicationRoutesController : Controller
{
    private readonly string IndexRender = "vue";
    private readonly IWebApplicationRouteWebService _webApplicationRouteWebService;
    public WebApplicationRoutesController(IWebApplicationRouteWebService webApplicationRouteWebService)
    {
        _webApplicationRouteWebService = webApplicationRouteWebService;
    }

    #region Regular MVC

    [HttpGet]
    public async Task<IActionResult> Index(int WebApplicationId)
    {
        if (IndexRender == "SSR")
        {
            var vm = await _webApplicationRouteWebService.GetIndexModel(WebApplicationId);
            return View("Views/WebApplications/Routes/Index.cshtml", vm);
        }
        else
        {
            var pm = await _webApplicationRouteWebService.GetPageModel(WebApplicationId);
            return View("Views/WebApplications/Routes/IndexWithVue.cshtml", pm);
        }
    }

    [AjaxOnly]
    [HttpGet("create")]
    public async Task<IActionResult> GetCreatePartial(int WebApplicationId)
    {
        var model = await _webApplicationRouteWebService.GetCreateRouteModel(WebApplicationId);
        return PartialView("Views/ProjectRequests/_CreateRequestPartial.cshtml", model);
    }

    [HttpGet("edit/{RouteId:int}")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> Edit(int RouteId, string tab = "overview")
    {
        var vm = await _webApplicationRouteWebService.GetViewModel(RouteId);
        ViewData["Tab"] = tab;
        return View("Views/WebApplications/Routes/Edit.cshtml", vm);
    }

    [HttpPost("edit/{RouteId:int}")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> Edit(int WebApplicationId, int RouteId, RouteViewModel vm)
    {
        return Ok(vm);
    }

    #endregion

    #region API

    [HttpGet("api")]
    public async Task<IActionResult> ApiIndex(int WebApplicationId)
    {
        var vm = await _webApplicationRouteWebService.GetIndexModel(WebApplicationId);
        return Ok(vm.Routes.Select(r => new
        {
            r.Id,
            r.Name,
            r.Method,
            r.Path,
            IntegrationType = (int)r.IntegrationType,
            r.Description
        }));
    }

    [HttpPost("api/create")]
    public async Task<IActionResult> ApiCreate(int WebApplicationId, RouteSaveModel request)
    {
        if (await _webApplicationRouteWebService.ValidateCreate(WebApplicationId, request, ModelState))
        {
            return Ok(new AjaxResult<RouteSaveModel>
            {
                Errors = new List<Error> { new("something went wrong") }
            });
        }
        int id = await _webApplicationRouteWebService.Create(WebApplicationId, request);
        return Ok(AjaxResult.Success());
    }

    [HttpGet("api/{RouteId:int}")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> ApiGet(int RouteId)
    {
        var vm = await _webApplicationRouteWebService.GetViewModel(RouteId);
        return Ok(new
        {
            vm.Id,
            vm.Name,
            vm.Method,
            vm.Path,
            vm.IntegrationType,
            Authorization = new
            {
                Type = (int)vm.Authorization.Type,
                Policies = vm.Authorization.PolicyIds
            }
        });
    }

    [HttpPost("api/edit/{RouteId:int}")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> ApiEdit(int RouteId, [FromBody]RouteSaveModel route)
    {
        return Ok(route);
        if (!await _webApplicationRouteWebService.ValidateEdit(RouteId, route, ModelState))
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            return BadRequest(allErrors);
        }
        await _webApplicationRouteWebService.Edit(RouteId, route);
        return NoContent();
    }

    [HttpPost("api/delete/{RouteId:int}")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> ApiDelete(int RouteId)
    {
        await _webApplicationRouteWebService.Delete(RouteId);
        return Ok();
    }

    [HttpGet("{RouteId:int}/mock-integration")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> MockIntegration(int RouteId)
    {
        var integration = await _webApplicationRouteWebService.GetMockIntegration(RouteId);
        return Ok(new AjaxResult<MockIntegrationViewModel>
        {
            Data = integration
        });
    }

    [HttpPost("api/{RouteId:int}/mock-integration")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> MockIntegration(int RouteId, MockIntegrationSaveModel integration)
    {
        await _webApplicationRouteWebService.SaveMockIntegration(RouteId, integration);
        return Ok(new AjaxResult<MockIntegrationSaveModel>
        {
            Data = integration
        });
    }
    #endregion
}