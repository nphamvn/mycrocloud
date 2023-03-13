using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockServer.Web.Filters;
using MockServer.Web.Models.WebApplications.Authorizations;
using MockServer.Web.Services;
using static MockServer.Web.Common.Constants;
namespace MockServer.Web.Controllers;

[Authorize]
[Route("webapps/{WebApplicationName}/authorization")]
[GetAuthUserWebApplicationId(RouteName.WebApplicationName, RouteName.WebApplicationId)]
public class WebApplicationAuthorizationsController : BaseController
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
        return View("Views/WebApplications/Authorization/Policy/Index.cshtml", model);
    }

    [HttpGet("policies/create")]
    public async Task<IActionResult> PolicyCreate(int WebApplicationId)
    {
        var model = await _webApplicationAuthorizationWebService.GetPolicyCreateModel(WebApplicationId);
        return View("Views/WebApplications/Authorization/Policy/Save.cshtml", model);
    }

    [HttpPost("policies/create")]
    public async Task<IActionResult> PolicyCreate(int WebApplicationId, PolicySaveModel model)
    {
        if (!ModelState.IsValid)
        {
            var temp = await _webApplicationAuthorizationWebService.GetPolicyCreateModel(WebApplicationId);
            model.WebApplication = temp.WebApplication;
            return View("Views/WebApplications/Authorization/Policy/Save.cshtml", model); 
        }
        await _webApplicationAuthorizationWebService.CreatePolicy(WebApplicationId, model);
        return RedirectToAction(nameof(PolicyIndex), Request.RouteValues);
    }

    [HttpGet("policies/{PolicyId:int}/edit")]
    public async Task<IActionResult> PolicyEdit(int PolicyId)
    {
        var model = await _webApplicationAuthorizationWebService.GetPolicyEditModel(PolicyId);
        return View("Views/WebApplications/Authorization/Policy/Save.cshtml", model);
    }

    [HttpPost("policies/{PolicyId:int}/edit")]
    public async Task<IActionResult> PolicyEdit(int PolicyId, PolicySaveModel model)
    {
        await _webApplicationAuthorizationWebService.EditPolicy(PolicyId, model);
        return RedirectToAction(nameof(PolicyIndex), Request.RouteValues);
    }
    
    [HttpPost("policies/{PolicyId:int}/delete")]
    public async Task<IActionResult> PolictDelete(int PolicyId)
    {
        await _webApplicationAuthorizationWebService.Delete(PolicyId);
        return RedirectToAction(nameof(PolicyIndex), Request.RouteValues);
    }
}