using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockServer.Web.Filters;
using MockServer.Web.Models.WebApplications;
using MockServer.Web.Services;
using static MockServer.Web.Common.Constants;

namespace MockServer.Web.Controllers;

[Authorize]
[Route("webapps")]
public class WebApplicationsController : BaseController
{
    private readonly IWebApplicationWebService _webApplicationWebService;

    public WebApplicationsController(IWebApplicationWebService webApplicationWebService)
    {
        _webApplicationWebService = webApplicationWebService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(WebApplicationIndexViewModel fm)
    {
        var vm = await _webApplicationWebService.GetIndexViewModel(fm.Search);
        return View("Views/WebApplications/WebApplications.Index.cshtml", vm);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        var vm = new WebApplicationCreateModel();
        return View("Views/WebApplications/WebApplications.Create.cshtml", vm);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(WebApplicationCreateModel app)
    {
        if (!ModelState.IsValid)
        {
            return View("Views/WebApplications/WebApplications.Create.cshtml", app);
        }
        await _webApplicationWebService.Create(app);

        return RedirectToAction(nameof(View), new { ProjectName = app.Name });
    }

    [HttpGet("{WebApplicationName}")]
    [GetAuthUserWebApplicationId(RouteName.WebApplicationName, RouteName.WebApplicationId)]
    public IActionResult Home(int WebApplicationId)
    {
        return RedirectToAction(nameof(Overview), Request.RouteValues);
    }

    [HttpGet("{WebApplicationName}/json")]
    [GetAuthUserWebApplicationId(RouteName.WebApplicationName, RouteName.WebApplicationId)]
    public async Task<IActionResult> GetAppJson(int WebApplicationId)
    {
        var vm = await _webApplicationWebService.Get(WebApplicationId);
        return Ok(vm);
    }

    [HttpGet("{WebApplicationName}/overview")]
    [GetAuthUserWebApplicationId(RouteName.WebApplicationName, RouteName.WebApplicationId)]
    public async Task<IActionResult> Overview(int WebApplicationId)
    {
        var vm = await _webApplicationWebService.Get(WebApplicationId);
        return View("Views/WebApplications/WebApplications.Overview.cshtml", vm);
    }
}