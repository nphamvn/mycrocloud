using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockServer.Web.Models.WebApplications;
using MockServer.Web.Services;
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
        return View("Views/Projects/Create.cshtml", vm);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(WebApplicationCreateModel app)
    {
        if (!ModelState.IsValid)
        {
            return View("Views/Projects/Create.cshtml", app);
        }
        await _webApplicationWebService.Create(app);

        return RedirectToAction(nameof(View), new { ProjectName = app.Name });
    }
}