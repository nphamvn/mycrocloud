using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockServer.Core.WebApplications;
using MockServer.Web.Attributes;
using MockServer.Web.Filters;
using MockServer.Web.Models.WebApplications.Routes;
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
    private readonly IConfiguration _configuration;

    public WebApplicationRoutesController(
        IWebApplicationRouteWebService webApplicationRouteWebService,
        IConfiguration configuration)
    {
        _webApplicationRouteWebService = webApplicationRouteWebService;
        _configuration = configuration;
    }

    #region Regular MVC

    [HttpGet]
    public async Task<IActionResult> Index(int WebApplicationId, string SearchTerm, string Sort)
    {
        if (IndexRender == "SSR")
        {
            var vm = await _webApplicationRouteWebService.GetIndexModel(WebApplicationId, SearchTerm, Sort);
            return View("/Views/WebApplications/Routes/Index.cshtml", vm);
        }
        else
        {
            var pm = await _webApplicationRouteWebService.GetPageModel(WebApplicationId, SearchTerm, Sort);
            return View("/Views/WebApplications/Routes/IndexWithVue.cshtml", pm);
        }
    }

    [AjaxOnly]
    [HttpGet("create")]
    public async Task<IActionResult> GetCreatePartial(int WebApplicationId)
    {
        var model = await _webApplicationRouteWebService.GetCreateRouteModel(WebApplicationId);
        return PartialView("/Views/WebApplications/WebApplications.Create.cshtml", model);
    }

    [HttpGet("edit/{RouteId:int}")]
    [ValidateRouteWebApplication(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> Edit(int RouteId, string tab = "overview")
    {
        var vm = await _webApplicationRouteWebService.GetViewModel(RouteId);
        ViewData["Tab"] = tab;
        return View("/Views/WebApplications/Routes/Edit.cshtml", vm);
    }

    [HttpPost("edit/{RouteId:int}")]
    [ValidateRouteWebApplication(RouteName.WebApplicationId, RouteName.RouteId)]
    public async Task<IActionResult> Edit(int WebApplicationId, int RouteId, RouteViewModel vm)
    {
        return Ok(vm);
    }

    #endregion
}