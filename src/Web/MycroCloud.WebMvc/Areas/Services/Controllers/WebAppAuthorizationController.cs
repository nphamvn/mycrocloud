using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MycroCloud.WebMvc.Areas.Services.Models.WebApps;
using MycroCloud.WebMvc.Areas.Services.Services;
using MycroCloud.WeMvc;

namespace MycroCloud.WebMvc.Areas.Services.Controllers;

[Authorize]
[Route("webapp/{WebAppName}/authorization")]
public class WebAppAuthorizationController(IWebAppAuthorizationService webAppAuthorizationService) : BaseServiceController
{
    public const string Name = "WebAppAuthorization";
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        base.OnActionExecuting(context);
        ViewData["WebAppName"] = context.ActionArguments["WebApplicationName"];
    }

    [HttpGet("Policies")]
    public async Task<IActionResult> PolicyList(int WebApplicationId)
    {
        var vm = await webAppAuthorizationService.GetPolicyListViewModel(WebApplicationId);
        return View("/Areas/Services/Views/WebApp/Authorization/PolicyList.cshtml", vm);
    }

    [HttpGet("Policies/Create")]
    public async Task<IActionResult> CreatePolicy(int WebApplicationId)
    {
        var model = await webAppAuthorizationService.GetPolicyCreateModel(WebApplicationId);
        ViewData["HeadTitle"] = "Create new policy";
        return View("/Areas/Services/Views/WebApp/Authorization/SavePolicy.cshtml", model);
    }

    [HttpPost("Policies/Create")]
    public async Task<IActionResult> CreatePolicy(string WebApplicationName, int WebApplicationId, AuthorizationPolicySaveViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var temp = await webAppAuthorizationService.GetPolicyCreateModel(WebApplicationId);
            return View("/Areas/Services/Views/WebApp/Authorization/SavePolicy.cshtml", viewModel);
        }
        await webAppAuthorizationService.CreatePolicy(WebApplicationId, viewModel);
        return RedirectToAction(nameof(PolicyList), new RouteValueDictionary
        {
            [Constants.RouteName.WebAppName] = WebApplicationName
        });
    }

    [HttpGet("Policies/{PolicyId:int}/Edit")]
    public async Task<IActionResult> EditPolicy(int PolicyId)
    {
        var model = await webAppAuthorizationService.GetPolicyEditModel(PolicyId);
        ViewData["HeadTitle"] = "Edit policy";
        return View("/Areas/Services/Views/WebApp/Authorization/SavePolicy.cshtml", model);
    }

    [HttpPost("Policies/{PolicyId:int}/Edit")]
    public async Task<IActionResult> EditPolicy(string WebApplicationName, int PolicyId, AuthorizationPolicySaveViewModel viewModel)
    {
        await webAppAuthorizationService.EditPolicy(PolicyId, viewModel);
        return RedirectToAction(nameof(PolicyList), new RouteValueDictionary
        {
            [Constants.RouteName.WebAppName] = WebApplicationName
        });
    }

    [HttpPost("Policies/{PolicyId:int}/Edit")]
    public async Task<IActionResult> DeletePolicy(string WebApplicationName, int PolicyId)
    {
        await webAppAuthorizationService.Delete(PolicyId);
        return RedirectToAction(nameof(PolicyList), new RouteValueDictionary
        {
            [Constants.RouteName.WebAppName] = WebApplicationName
        });
    }
}
