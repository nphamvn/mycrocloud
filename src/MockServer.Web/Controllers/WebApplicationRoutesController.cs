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
    private readonly IWebApplicationRouteWebService _webApplicationRouteWebService;
    public WebApplicationRoutesController(IWebApplicationRouteWebService webApplicationRouteWebService)
    {
        _webApplicationRouteWebService = webApplicationRouteWebService;
    }

    public async Task<IActionResult> Index(int WebApplicationId)
    {
        var vm = await _webApplicationRouteWebService.GetIndexModel(WebApplicationId);
        return View("Views/WebApplications/Routes/Index.cshtml", vm);
    }

    [HttpGet("api")]
    public async Task<IActionResult> AjaxIndex(int WebApplicationId)
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

    [HttpGet("blazor")]
    public async Task<IActionResult> IndexWithBlazor(int WebApplicationId)
    {
        var vm = await _webApplicationRouteWebService.GetIndexModel(WebApplicationId);
        return View("Views/WebApplications/Routes/IndexBlazor.cshtml", vm);
    }

    [HttpGet("vue")]
    public async Task<IActionResult> IndexWithVue(int WebApplicationId)
    {
        var vm = await _webApplicationRouteWebService.GetIndexModel(WebApplicationId);
        return View("Views/WebApplications/Routes/IndexWithVue.cshtml", vm);
    }

    [AjaxOnly]
    public async Task<IActionResult> IndexKnockoutAjax(int WebApplicationId)
    {
        var vm = await _webApplicationRouteWebService.GetIndexModel(WebApplicationId);
        return Ok(vm.Routes);
    }

    [HttpGet("ko")]
    public async Task<IActionResult> KnockoutIndex(int WebApplicationId)
    {
        var vm = await _webApplicationRouteWebService.GetIndexModel(WebApplicationId);
        return View("Views/WebApplications/Routes/IndexKo.cshtml", vm);
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
    [HttpGet("{RouteId:int}/details")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> AjaxIndexDetails(int RouteId)
    {
        var vm = await _webApplicationRouteWebService.GetViewModel(RouteId);
        return PartialView("Views/WebApplications/Routes/_IndexRouteDetails.cshtml", vm);
    }

    [HttpGet("edit/{RouteId:int}")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> Edit(int RouteId, string tab = "overview")
    {
        var vm = await _webApplicationRouteWebService.GetViewModel(RouteId);
        ViewData["Tab"] = tab;
        return View("Views/WebApplications/Routes/Details.cshtml", vm);
    }

    [HttpPost("edit/{RouteId:int}")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> ChangeIntegrationType(int RouteId, RouteIntegrationType Type)
    {
        await _webApplicationRouteWebService.ChangeIntegrationType(RouteId, Type);
        return RedirectToAction(nameof(Edit), Request.RouteValues);
    }

    [HttpGet("api/{RouteId:int}")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> GetRouteJson(int RouteId)
    {
        var vm = await _webApplicationRouteWebService.GetViewModel(RouteId);
        return Ok(new {
            vm.Id,
            vm.Name,
            vm.Method,
            vm.Path,
            Url = "vm.Url",
            vm.IntegrationType,
            Authorization = new {
                Type = (int)vm.Authorization.Type,
                Policies = vm.Authorization.PolicyIds
            }
        });
    }

    [AjaxOnly]
    [HttpPost("{RouteId:int}")]
    [ValidateProjectRequest(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> Edit(int WebApplicationId, int RouteId, RouteViewModel vm)
    {
        return Ok(vm);
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