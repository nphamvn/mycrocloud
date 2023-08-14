using MycroCloud.WebMvc.Areas.Services.Models.WebApps;
using Microsoft.AspNetCore.Mvc;
using MycroCloud.WebMvc.Areas.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using MycroCloud.WebMvc.Areas.Services.Authorization;
using MycroCloud.WebMvc.Identity;

namespace MycroCloud.WebMvc.Areas.Services.Controllers;

public class WebAppController(IWebAppService webAppService
    , IAuthorizationService authorizationService
    , ILogger<WebAppController> logger) : BaseServiceController
{
    public const string Name = "WebApp";

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Index(WebAppSearchModel fm)
    {
        var vm = await webAppService.GetIndexViewModel(fm, MycroCloudUser.Id);
        return View("/Areas/Services/Views/WebApp/Index.cshtml", vm);
    }

    [HttpGet("Create")]
    public IActionResult Create()
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

        await webAppService.Create(app, MycroCloudUser.Id);
        return RedirectToAction(nameof(View), new { WebAppName = app.Name });
    }

    [AllowAnonymous]
    [HttpGet("{WebAppName}")]
    public async Task<IActionResult> View(string WebAppName)
    {
        var app = await webAppService.FindByUserIdAndAppName(MycroCloudUser.Id, WebAppName);
        if (app == null) return NotFound();
        var authResult = await authorizationService.AuthorizeAsync(User, app,
            WebAppAuthorizationHandler.Operations.View);
        if (!authResult.Succeeded) return NotFound();
        
        var vm = await webAppService.GetViewViewModel(app.WebAppId);
        return View("/Areas/Services/Views/WebApp/View.cshtml", vm);
    }

    [HttpPost("{WebAppName}/Rename")]
    public async Task<IActionResult> Rename(string WebAppName, WebAppRenameRequestModel renameRequestModel)
    {
        var app = await webAppService.FindByUserIdAndAppName(MycroCloudUser.Id, WebAppName);
        if (app == null) return NotFound();
        var authResult = await authorizationService.AuthorizeAsync(User, app,
            WebAppAuthorizationHandler.Operations.View);
        if (!authResult.Succeeded) return NotFound();
        
        await webAppService.Rename(app.WebAppId, renameRequestModel.Name);
        return RedirectToAction(nameof(View), new { WebAppName = renameRequestModel.Name });
    }

    [HttpPost("{WebAppName}/Delete")]
    public async Task<IActionResult> Delete(string WebAppName)
    {
        var app = await webAppService.FindByUserIdAndAppName(MycroCloudUser.Id, WebAppName);
        if (app == null) return NotFound();
        var authResult = await authorizationService.AuthorizeAsync(User, app,
            WebAppAuthorizationHandler.Operations.View);
        if (!authResult.Succeeded) return MycroCloudUser != null ? Forbid() : NotFound();
        
        await webAppService.Delete(app.WebAppId);
        return RedirectToAction(nameof(Index));
    }
}

public class WebAppAuthorization(IAuthorizationService authorizationService
    , IWebAppService webAppService) : IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;
        var serviceOwner = context.HttpContext.Items["ServiceOwner"] as MycroCloudIdentityUser;
        ArgumentNullException.ThrowIfNull(serviceOwner);
        var appName = context.RouteData.Values["WebAppName"] as string;
        var app = await webAppService.FindByUserIdAndAppName(serviceOwner.Id, appName);
        var action = context.RouteData.Values["Action"] as string;
        var authorizationResult = action switch
        {
            nameof(WebAppController.Index) => AuthorizationResult.Success(),
            nameof(WebAppController.Create) => AuthorizationResult.Success(),
            nameof(WebAppController.View) => await authorizationService.AuthorizeAsync(user, app,
                WebAppAuthorizationHandler.Operations.View),
            nameof(WebAppController.Rename) => await authorizationService.AuthorizeAsync(user, app,
                WebAppAuthorizationHandler.Operations.Rename),
            nameof(WebAppController.Delete) => await authorizationService.AuthorizeAsync(user, app,
                WebAppAuthorizationHandler.Operations.Delete),
            _ => throw new Exception()
        };
        
        if (authorizationResult is { Succeeded: false })
        {
            if (user.Identity is { IsAuthenticated: true })
            {
                context.Result = new ForbidResult();
            }
            else
            {
                context.Result = new ChallengeResult();
            }
        }
    }
}