using MycroCloud.WebMvc.Areas.Services.Models.WebApps;
using Microsoft.AspNetCore.Mvc;
using MycroCloud.WebMvc.Areas.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Identity;
using MycroCloud.WebMvc.Areas.Services.Authorization;

namespace MycroCloud.WebMvc.Areas.Services.Controllers;
[Authorize]
public class WebAppsController(IWebAppService webAppService
    , IAuthorizationService authorizationService
    , ILogger<WebAppsController> logger) : BaseServiceController
{
    private readonly IWebAppService _webAppService = webAppService;
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private readonly ILogger<WebAppsController> _logger = logger;

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await base.OnActionExecutionAsync(context, next);

        var appName = context.RouteData.Values["WebAppName"] as string;
        WebAppModel app = default;
        if (ServiceOwner is not IdentityUser owner)
        {
            _logger.LogInformation("ServiceOwner not found");
            context.Result = new NotFoundResult();
            return;
        }

        if (!string.IsNullOrEmpty(appName))
        {
            app = await _webAppService.Find(owner.Id, appName.ToString());
            if (app == null)
            {
                _logger.LogInformation("WebApp not found");
                context.Result = new NotFoundResult();
                return;
            }
            context.ActionArguments["WebAppId"] = app.WebAppId;
            context.HttpContext.Items["WebApp"] = app;
        }
        //Authorization
        AuthorizationResult authorizationResult = default;
        var action = context.ActionDescriptor.RouteValues["action"];
        switch (action)
        {
            case nameof(Index):
                authorizationResult = await _authorizationService.AuthorizeAsync(User, app, WebAppAuthorizationHandler.Operations.Index);
                break;
            case nameof(Create):
                authorizationResult = await _authorizationService.AuthorizeAsync(User, app, WebAppAuthorizationHandler.Operations.Create);
                break;
            case nameof(View):
                authorizationResult = await _authorizationService.AuthorizeAsync(User, app, WebAppAuthorizationHandler.Operations.View);
                break;
            case nameof(Rename):
                authorizationResult = await _authorizationService.AuthorizeAsync(User, app, WebAppAuthorizationHandler.Operations.Rename);
                break;
            case nameof(Delete):
                authorizationResult = await _authorizationService.AuthorizeAsync(User, app, WebAppAuthorizationHandler.Operations.Delete);
                break;
            default:
                break;
        }
        if (!authorizationResult.Succeeded)
        {
            if (User.Identity.IsAuthenticated)
            {
                context.Result = new ForbidResult();
            }
            else
            {
                context.Result = new ChallengeResult();
            }
            return;
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
    public async Task<IActionResult> Create()
    {
        var vm = new WebAppCreateViewModel();
        return View("/Areas/Services/Views/WebApp/Create.cshtml", vm);
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

    [HttpPost("Rename")]
    public async Task<IActionResult> Rename(int WebAppId, string newName)
    {
        await _webAppService.Rename(WebAppId, newName);
        return RedirectToAction(nameof(Index), new { WebAppName = newName });
    }

    [HttpPost("Delete")]
    public async Task<IActionResult> Delete(int WebAppId)
    {
        await _webAppService.Delete(WebAppId);
        return RedirectToAction(nameof(Index));
    }
}