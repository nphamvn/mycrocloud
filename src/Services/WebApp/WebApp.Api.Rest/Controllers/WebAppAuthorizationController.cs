using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MycroCloud.WebApp.Api.Rest.Controllers;

[Authorize]
[Route("webapps/{WebApplicationName}/authorization")]
public class WebApplicationAuthorizationsController : BaseApiController
{
    private readonly IWebApplicationAuthorizationWebService _webApplicationAuthorizationWebService;

    public WebApplicationAuthorizationsController(IWebApplicationAuthorizationWebService webApplicationAuthorizationWebService)
    {
        _webApplicationAuthorizationWebService = webApplicationAuthorizationWebService;
    }

    [HttpGet("policies")]
    public async Task<IActionResult> PolicyIndex(int WebApplicationId)
    {
        var model = await _webApplicationAuthorizationWebService.GetPolicyIndexViewModel(WebApplicationId);
        return View("/Views/WebApplications/Authorization/Policy/Index.cshtml", model);
    }

    [HttpGet("policies/create")]
    public async Task<IActionResult> PolicyCreate(int WebApplicationId)
    {
        var model = await _webApplicationAuthorizationWebService.GetPolicyCreateModel(WebApplicationId);
        ViewData["HeadTitle"] = "Create new policy";
        return View("/Views/WebApplications/Authorization/Policy/Save.cshtml", model);
    }

    [HttpPost("policies/create")]
    public async Task<IActionResult> PolicyCreate(string WebApplicationName, int WebApplicationId, PolicySaveModel model)
    {
        if (!ModelState.IsValid)
        {
            var temp = await _webApplicationAuthorizationWebService.GetPolicyCreateModel(WebApplicationId);
            model.WebApplication = temp.WebApplication;
            return View("/Views/WebApplications/Authorization/Policy/Save.cshtml", model);
        }
        await _webApplicationAuthorizationWebService.CreatePolicy(WebApplicationId, model);
        return RedirectToAction(nameof(PolicyIndex), new RouteValueDictionary
        {
            [Constants.RouteName.WebApplicationName] = WebApplicationName
        });
    }

    [HttpGet("policies/{PolicyId:int}/edit")]
    public async Task<IActionResult> PolicyEdit(int PolicyId)
    {
        var model = await _webApplicationAuthorizationWebService.GetPolicyEditModel(PolicyId);
        ViewData["HeadTitle"] = "Edit policy";
        return View("/Views/WebApplications/Authorization/Policy/Save.cshtml", model);
    }

    [HttpPost("policies/{PolicyId:int}/edit")]
    public async Task<IActionResult> PolicyEdit(string WebApplicationName, int PolicyId, PolicySaveModel model)
    {
        await _webApplicationAuthorizationWebService.EditPolicy(PolicyId, model);
        return RedirectToAction(nameof(PolicyIndex), new RouteValueDictionary
        {
            [Constants.RouteName.WebApplicationName] = WebApplicationName
        });
    }

    [HttpPost("policies/{PolicyId:int}/delete")]
    public async Task<IActionResult> PolictDelete(string WebApplicationName, int PolicyId)
    {
        await _webApplicationAuthorizationWebService.Delete(PolicyId);
        return RedirectToAction(nameof(PolicyIndex), new RouteValueDictionary
        {
            [Constants.RouteName.WebApplicationName] = WebApplicationName
        });
    }
}
