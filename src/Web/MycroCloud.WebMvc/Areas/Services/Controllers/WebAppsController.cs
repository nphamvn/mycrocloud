using MicroCloud.Web.Common;
using MicroCloud.Web.Controllers;
using MicroCloud.Web.Filters;
using MicroCloud.Web.Services;
using MicroCloud.WebMvc.Areas.Services.Models.WebApp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MycroCloud.Web.Controllers;

namespace MycroCloud.WebMvc.Areas.Services.Controllers;

[Authorize]
[Route("webapps")]
[GetAuthUserWebApplicationId(Constants.RouteName.WebApplicationName, Constants.RouteName.WebApplicationId)]
public class WebAppsController : BaseController
{
    private readonly IWebApplicationService _webApplicationWebService;

    public WebAppsController(IWebApplicationService webApplicationWebService)
    {
        _webApplicationWebService = webApplicationWebService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(WebAppIndexViewModel fm)
    {
        var vm = await _webApplicationWebService.GetIndexViewModel(fm.Search);
        return View("/Views/WebApplications/Index.cshtml", vm);
    }

    [HttpGet("create")]
    public Task<IActionResult> Create()
    {
        var vm = new WebAppCreateViewModel();
        return Task.FromResult<IActionResult>(View("/Views/WebApplications/Create.cshtml", vm));
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(WebAppCreateViewModel app)
    {
        if (!ModelState.IsValid)
        {
            return View("/Views/WebApplications/Create.cshtml", app);
        }
        await _webApplicationWebService.Create(app);

        return RedirectToAction(nameof(Overview), new { WebApplicationName = app.Name });
    }

    [HttpGet("{WebApplicationName}")]
    [HttpGet("{WebApplicationName}/overview")]
    public async Task<IActionResult> Overview(int WebApplicationId)
    {
        var vm = await _webApplicationWebService.Get(WebApplicationId);
        return View("/Views/WebApplications/Overview.cshtml", vm);
    }
    
    [HttpPost("rename")]
    public async Task<IActionResult> Rename(int WebApplicationId, string newName)
    {
        await _webApplicationWebService.Rename(WebApplicationId, newName);
        return RedirectToAction(nameof(Index), new { WebApplicationName = newName });
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete(int WebApplicationId)
    {
        await _webApplicationWebService.Delete(WebApplicationId);
        return RedirectToAction(nameof(Index));
    }
}