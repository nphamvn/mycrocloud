using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
[Route("webapps/{AppName}/routes")]
[GetAuthUserWebApplicationId(RouteName.WebApplicationName, RouteName.WebApplicationId)]
public class WebApplicationRoutesController : Controller
{
    private readonly IWebApplicationRouteWebService _webApplicationRouteWebService;
    public WebApplicationRoutesController(IWebApplicationRouteWebService webApplicationRouteWebService)
    {
        _webApplicationRouteWebService = webApplicationRouteWebService;
    }

    [HttpGet("/webapps/{AppName}")]
    [GetAuthUserWebApplicationId(RouteName.WebApplicationName, RouteName.WebApplicationId)]
    public async Task<IActionResult> Index(int ProjectId)
    {
        var vm = await _webApplicationRouteWebService.GetIndexModel(ProjectId);
        return View("Views/WebApplications/Routes/Index.cshtml", vm);
    }

    [AjaxOnly]
    [HttpGet("create")]
    public async Task<IActionResult> GetCreatePartial(int ProjectId)
    {
        var model = await _webApplicationRouteWebService.GetCreateRouteModel(ProjectId);
        return PartialView("Views/ProjectRequests/_CreateRequestPartial.cshtml", model);
    }

    [AjaxOnly]
    [HttpPost("create")]
    public async Task<IActionResult> CreateAjax(int ProjectId, RouteSaveModel request)
    {
        if (await _webApplicationRouteWebService.ValidateCreate(ProjectId, request, ModelState))
        {
            return Ok(new AjaxResult<RouteSaveModel>
            {
                Errors = new List<Error>{ new("something went wrong") }
            });
        }
        int id = await _webApplicationRouteWebService.Create(ProjectId, request);
        return Ok(AjaxResult.Success());
    }

    [AjaxOnly]
    [HttpGet("{RequestId:int}/edit")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> GetEditPartial(int ProjectId, int RequestId)
    {
        var vm = await _webApplicationRouteWebService.GetEditRouteModel(RequestId);
        ViewData["FormMode"] = "Edit";
        return PartialView("Views/Requests/_CreateRequestPartial.cshtml", vm);
    }

    [AjaxOnly]
    [HttpPost("{RequestId:int}/edit")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> Edit(int RequestId, RouteSaveModel request)
    {
        if (!await _webApplicationRouteWebService.ValidateEdit(RequestId, request, ModelState))
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            return BadRequest(allErrors);
        }
        await _webApplicationRouteWebService.Edit(RequestId, request);
        return NoContent();
    }

    [AjaxOnly]
    [HttpGet("{RequestId:int}")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> View(int RequestId)
    {
        var vm = await _webApplicationRouteWebService.GetViewModel(RequestId);
        return PartialView("Views/Requests/_RequestOpen.cshtml", vm);
    }

    [AjaxOnly]
    [HttpGet("{RequestId:int}/authorization")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> GetAuthorizationPartial(int ProjectId, int id)
    {
        var vm = await _webApplicationRouteWebService.GetAuthorizationViewModel(ProjectId, id);
        return PartialView("Views/Requests/_AuthorizationConfigPartial.cshtml", vm);
    }

    [AjaxOnly]
    [HttpPost("{RequestId:int}/authorization")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> ConfigAuthorization(int ProjectId, int RequestId, AuthorizationSaveModel auth)
    {
        await _webApplicationRouteWebService.AttachAuthorization(RequestId, auth);
        return NoContent();
    }

    [AjaxOnly]
    [HttpPost("{RequestId:int}/mock-integration")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> SaveRequestConfiguration(int RequestId, MockIntegrationSaveModel config)
    {
        await _webApplicationRouteWebService.SaveMockIntegration(RequestId, config);
        return Ok(new AjaxResult<MockIntegrationSaveModel>
        {
            Data = config
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