using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockServer.Core.WebApplications;
using MockServer.Web.Filters;
using MockServer.Web.Services;
using RouteName = MockServer.Web.Common.Constants.RouteName;
namespace MockServer.Web.Controllers;

[Authorize]
[Route("webapps/{ProjectName}/settings")]
[GetAuthUserWebApplicationId(RouteName.WebApplicationName, RouteName.WebApplicationId)]
public class WebApplicationSettingsController : BaseController
{
    private readonly IWebApplicationWebService _webApplicationWebService;
    public WebApplicationSettingsController(IWebApplicationWebService webApplicationWebService)
    {
        _webApplicationWebService = webApplicationWebService;
    }

    [HttpGet]
    //[HttpGet("general")]
    public async Task<IActionResult> Index(int ProjectId)
    {
        var model = await _webApplicationWebService.Get(ProjectId);
        return View("Views/ProjectSettings/Index.cshtml", model);
    }

    [HttpPost("rename")]
    public async Task<IActionResult> Rename(int ProjectId, string newName)
    {
        await _webApplicationWebService.Rename(ProjectId, newName);
        return RedirectToAction(nameof(Index), new { name = newName });
    }

    [HttpPost("set-accessibility")]
    public async Task<IActionResult> SetAccessibility(int ProjectId, WebApplicationAccessibility accessibility)
    {
        await _webApplicationWebService.SetAccessibility(ProjectId, accessibility);
        return RedirectToAction(nameof(View), Request.RouteValues);
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete(int ProjectId)
    {
        await _webApplicationWebService.Delete(ProjectId);
        return RedirectToAction(nameof(Index));
    }
}