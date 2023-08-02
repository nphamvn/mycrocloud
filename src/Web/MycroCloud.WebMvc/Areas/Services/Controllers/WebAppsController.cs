using MycroCloud.WebMvc.Areas.Services.Models.WebApps;
using Microsoft.AspNetCore.Mvc;
using MycroCloud.WebMvc.Areas.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;

namespace MycroCloud.WebMvc.Areas.Services.Controllers;
[Authorize]
public class WebAppsController(IWebAppService webAppService, ILogger<WebAppsController> logger) : BaseServiceController
{
    private readonly IWebAppService _webAppService = webAppService;
    private readonly ILogger<WebAppsController> _logger = logger;

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await base.OnActionExecutionAsync(context, next);
        if (ServiceOwner is not IdentityUser owner)
        {
            _logger.LogInformation("ServiceOwner not found");
            context.Result = new NotFoundResult();
            return;
        }
        if (context.ActionArguments.TryGetValue("WebAppName", out object appName))
        {
            var webapp = await _webAppService.Find(owner.Id, appName.ToString());
            if (webapp == null)
            {
                _logger.LogInformation("WebApp not found");
                context.Result = new NotFoundResult();
                return;
            }
            context.ActionArguments["WebAppId"] = webapp.WebAppId;
            context.HttpContext.Items["WebApp"] = webapp;
        }
        await next();
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Index(WebAppSearchModel fm)
    {
        var vm = await _webAppService.GetIndexViewModel(fm);
        return View("/Areas/Services/Views/WebApp/Index.cshtml", vm);
    }

    [HttpGet("Create")]
    public Task<IActionResult> Create()
    {
        var vm = new WebAppCreateViewModel();
        return Task.FromResult<IActionResult>(View("/Areas/Services/Views/WebApp/Create.cshtml", vm));
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(WebAppCreateViewModel app)
    {
        if (!ModelState.IsValid)
        {
            return View("/Areas/Services/Views/WebApp/Create.cshtml", app);
        }
        await _webAppService.Create(app);

        return RedirectToAction(nameof(View), new { WebAppName = app.Name });
    }

    [AllowAnonymous]
    [HttpGet("{WebAppName}")]
    public async Task<IActionResult> View(int WebAppId)
    {
        var vm = await _webAppService.Get(WebAppId);
        return View("/Areas/Services/Views/WebApp/View.cshtml", vm);
    }

    [HttpPost("rename")]
    public async Task<IActionResult> Rename(int WebAppId, string newName)
    {
        await _webAppService.Rename(WebAppId, newName);
        return RedirectToAction(nameof(Index), new { WebAppName = newName });
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete(int WebAppId)
    {
        await _webAppService.Delete(WebAppId);
        return RedirectToAction(nameof(Index));
    }
}