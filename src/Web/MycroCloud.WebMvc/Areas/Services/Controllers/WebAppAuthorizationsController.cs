using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MycroCloud.WebMvc.Areas.Services.Models.WebApps;
using MycroCloud.WebMvc.Areas.Services.Services;
using MycroCloud.WeMvc;

namespace MycroCloud.WebMvc.Areas.Services.Controllers;

[Authorize]
[Route("webapps/{WebApplicationName}/authorizations")]
public class WebAppAuthorizationsController(IWebAppAuthorizationService webAppAuthorizationService) : BaseServiceController
{
    private readonly IWebAppAuthorizationService _webAppAuthorizationService = webAppAuthorizationService;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ViewData["WebAppName"] = context.ActionArguments["WebApplicationName"];
    }

    [HttpGet("Policies")]
    public async Task<IActionResult> PolicyList(int WebApplicationId)
    {
        var vm = await _webAppAuthorizationService.GetPolicyListViewModel(WebApplicationId);
        return View("/Areas/Services/Views/WebApp/Authorization/PolicyList.cshtml", vm);
    }

    [HttpGet("Policies/Create")]
    public async Task<IActionResult> CreatePolicy(int WebApplicationId)
    {
        var model = await _webAppAuthorizationService.GetPolicyCreateModel(WebApplicationId);
        ViewData["HeadTitle"] = "Create new policy";
        return View("/Areas/Services/Views/WebApp/Authorization/SavePolicy.cshtml", model);
    }

    [HttpPost("Policies/Create")]
    public async Task<IActionResult> CreatePolicy(string WebApplicationName, int WebApplicationId, AuthorizationPolicySaveViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            var temp = await _webAppAuthorizationService.GetPolicyCreateModel(WebApplicationId);
            return View("/Areas/Services/Views/WebApp/Authorization/SavePolicy.cshtml", viewModel);
        }
        await _webAppAuthorizationService.CreatePolicy(WebApplicationId, viewModel);
        return RedirectToAction(nameof(PolicyList), new RouteValueDictionary
        {
            [Constants.RouteName.WebAppName] = WebApplicationName
        });
    }

    [HttpGet("Policies/{PolicyId:int}/Edit")]
    public async Task<IActionResult> EditPolicy(int PolicyId)
    {
        var model = await _webAppAuthorizationService.GetPolicyEditModel(PolicyId);
        ViewData["HeadTitle"] = "Edit policy";
        return View("/Areas/Services/Views/WebApp/Authorization/SavePolicy.cshtml", model);
    }

    [HttpPost("Policies/{PolicyId:int}/Edit")]
    public async Task<IActionResult> EditPolicy(string WebApplicationName, int PolicyId, AuthorizationPolicySaveViewModel viewModel)
    {
        await _webAppAuthorizationService.EditPolicy(PolicyId, viewModel);
        return RedirectToAction(nameof(PolicyList), new RouteValueDictionary
        {
            [Constants.RouteName.WebAppName] = WebApplicationName
        });
    }

    [HttpPost("Policies/{PolicyId:int}/Edit")]
    public async Task<IActionResult> DeletePolicy(string WebApplicationName, int PolicyId)
    {
        await _webAppAuthorizationService.Delete(PolicyId);
        return RedirectToAction(nameof(PolicyList), new RouteValueDictionary
        {
            [Constants.RouteName.WebAppName] = WebApplicationName
        });
    }
}
