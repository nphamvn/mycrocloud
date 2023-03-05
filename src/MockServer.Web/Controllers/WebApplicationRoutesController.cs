using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MockServer.Core.WebApplications;
using MockServer.Web.Attributes;
using MockServer.Web.Filters;
using MockServer.Web.Models.Common;
using MockServer.Web.Models.WebApplications.Routes;
using MockServer.Web.Models.WebApplications.Routes.Authorizations;
using MockServer.Web.Models.WebApplications.Routes.Integrations.MockIntegrations;
using MockServer.Web.Services;
using RouteName = MockServer.Web.Common.Constants.RouteName;
namespace MockServer.Web.Controllers;

[Authorize]
[Route("webapps/{WebApplicationName}/routes")]
[GetAuthUserWebApplicationId(RouteName.WebApplicationName, RouteName.WebApplicationId)]
public class WebApplicationRoutesController : Controller
{
    private readonly IWebApplicationRouteWebService _webApplicationRouteWebService;
    public WebApplicationRoutesController(IWebApplicationRouteWebService webApplicationRouteWebService)
    {
        _webApplicationRouteWebService = webApplicationRouteWebService;
    }

    [GetAuthUserWebApplicationId(RouteName.WebApplicationName, RouteName.WebApplicationId)]
    public async Task<IActionResult> Index(int WebApplicationId)
    {
        var vm = await _webApplicationRouteWebService.GetIndexModel(WebApplicationId);
        return View("Views/WebApplications/Routes/Index.cshtml", vm);
    }

    [AjaxOnly]
    [HttpGet("create")]
    public async Task<IActionResult> GetCreatePartial(int WebApplicationId)
    {
        var model = await _webApplicationRouteWebService.GetCreateRouteModel(WebApplicationId);
        return PartialView("Views/ProjectRequests/_CreateRequestPartial.cshtml", model);
    }

    [AjaxOnly]
    [HttpPost("create")]
    public async Task<IActionResult> CreateAjax(int WebApplicationId, RouteSaveModel request)
    {
        if (await _webApplicationRouteWebService.ValidateCreate(WebApplicationId, request, ModelState))
        {
            return Ok(new AjaxResult<RouteSaveModel>
            {
                Errors = new List<Error>{ new("something went wrong") }
            });
        }
        int id = await _webApplicationRouteWebService.Create(WebApplicationId, request);
        return Ok(AjaxResult.Success());
    }

    [AjaxOnly]
    [HttpGet("{RouteId:int}/edit")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> GetEditPartial(int WebApplicationId, int RouteId)
    {
        var vm = await _webApplicationRouteWebService.GetEditRouteModel(RouteId);
        ViewData["FormMode"] = "Edit";
        return PartialView("Views/Requests/_CreateRequestPartial.cshtml", vm);
    }

    [AjaxOnly]
    [HttpPost("{RouteId:int}/edit")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> Edit(int RouteId, RouteSaveModel request)
    {
        if (!await _webApplicationRouteWebService.ValidateEdit(RouteId, request, ModelState))
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            return BadRequest(allErrors);
        }
        await _webApplicationRouteWebService.Edit(RouteId, request);
        return NoContent();
    }

    [AjaxOnly]
    [HttpGet("{RouteId:int}")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> View(int RouteId)
    {
        var vm = await _webApplicationRouteWebService.GetViewModel(RouteId);
        return PartialView("Views/Requests/_RequestOpen.cshtml", vm);
    }

    [HttpGet("{RouteId:int}")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> Details(int RouteId)
    {
        var vm = await _webApplicationRouteWebService.GetViewModel(RouteId);
        return View("Views/WebApplications/Routes/Details.cshtml", vm);
    }

    [HttpPost("{RouteId:int}/change-integration-type")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> ChangeIntegrationType(int RouteId, RouteIntegrationType Type)
    {
        await _webApplicationRouteWebService.ChangeIntegrationType(RouteId, Type);
        return RedirectToAction(nameof(Details), Request.RouteValues);
    }

    [AjaxOnly]
    [HttpGet("{RouteId:int}/authorization")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> GetAuthorizationPartial(int WebApplicationId, int id)
    {
        var vm = await _webApplicationRouteWebService.GetAuthorizationViewModel(WebApplicationId, id);
        return PartialView("Views/Requests/_AuthorizationConfigPartial.cshtml", vm);
    }

    [AjaxOnly]
    [HttpPost("{RouteId:int}/authorization")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> ConfigAuthorization(int WebApplicationId, int RouteId, AuthorizationSaveModel auth)
    {
        await _webApplicationRouteWebService.AttachAuthorization(RouteId, auth);
        return Ok(new AjaxResult<AuthorizationSaveModel>
        {
            Data = auth
        });
    }

    //[AjaxOnly]
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

    [AjaxOnly]
    [HttpPost("{RouteId:int}/mock-integration")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> MockIntegration(int RouteId, MockIntegrationSaveModel integration)
    {
        await _webApplicationRouteWebService.SaveMockIntegration(RouteId, integration);
        return Ok(new AjaxResult<MockIntegrationSaveModel>
        {
            Data = integration
        });
    }

    [AjaxOnly]
    [HttpPost("{RequestId:int}/delete")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> Delete(int RequestId)
    {
        await _webApplicationRouteWebService.Delete(RequestId);
        return Ok();
    }
}