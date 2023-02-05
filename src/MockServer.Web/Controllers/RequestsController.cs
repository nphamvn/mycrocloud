using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MockServer.Web.Attributes;
using MockServer.Web.Filters;
using MockServer.Web.Models.Requests;
using MockServer.Web.Services.Interfaces;
using RouteName = MockServer.Web.Common.Constants.RouteName;
namespace MockServer.Web.Controllers;

[Authorize]
[Route("projects/{ProjectName}/requests")]
[GetAuthUserProjectId(RouteName.ProjectName, RouteName.ProjectId)]
public class RequestsController : Controller
{
    private readonly IRequestWebService _requestService;
    public RequestsController(IRequestWebService requestService)
    {
        _requestService = requestService;
    }

    [AjaxOnly]
    [HttpGet("{RequestId:int}")]
    [ValidateProjectRequest(RouteName.ProjectId, RouteName.RequestId)]
    public async Task<IActionResult> Open(int ProjectId, int RequestId)
    {
        var vm = await _requestService.GetRequestOpenViewModel(RequestId);
        return PartialView("Views/Requests/_RequestOpen.cshtml", vm);
    }

    [AjaxOnly]
    [HttpGet("create")]
    public async Task<IActionResult> GetCreatePartial(int ProjectId)
    {
        var model = await _requestService.GetCreateRequestViewModel(ProjectId);
        return PartialView("Views/Requests/_CreateRequestPartial.cshtml", model);
    }

    [AjaxOnly]
    [HttpPost("create")]
    public async Task<IActionResult> CreateAjax(int ProjectId, CreateUpdateRequestViewModel request)
    {
        if (!ModelState.IsValid)
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            return BadRequest(allErrors);
        }
        int id = await _requestService.Create(ProjectId, request);
        return Ok(new
        {
            id = id
        });
    }

    [AjaxOnly]
    [HttpGet("{RequestId:int}/edit")]
    [ValidateProjectRequest(RouteName.ProjectId, RouteName.RequestId)]
    public async Task<IActionResult> GetEditPartial(int ProjectId, int RequestId)
    {
        var vm = await _requestService.GetCreateRequestViewModel(RequestId);
        ViewData["FormMode"] = "Edit";
        return PartialView("Views/Requests/_CreateRequestPartial.cshtml", vm);
    }

    [AjaxOnly]
    [HttpPost("{RequestId:int}/edit")]
    [ValidateProjectRequest(RouteName.ProjectId, RouteName.RequestId)]
    public async Task<IActionResult> Edit(int RequestId, CreateUpdateRequestViewModel request)
    {
        if (!await _requestService.ValidateEdit(RequestId, request, ModelState))
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            return BadRequest(allErrors);
        }
        await _requestService.Edit(RequestId, request);
        return NoContent();
    }

    [AjaxOnly]
    [HttpGet("{id:int}/authorization")]
    [ValidateProjectRequest(RouteName.ProjectId, RouteName.RequestId)]
    public async Task<IActionResult> GetAuthorizationPartial(int ProjectId, int id)
    {
        var vm = await _requestService.GetAuthorizationConfigViewModel(ProjectId, id);
        return PartialView("Views/Requests/_AuthorizationConfigPartial.cshtml", vm);
    }

    [AjaxOnly]
    [HttpPost("{RequestId:int}/authorization")]
    [ValidateProjectRequest(RouteName.ProjectId, RouteName.RequestId)]
    public async Task<IActionResult> ConfigAuthorization(int ProjectId, int RequestId, AuthorizationConfigViewModel auth)
    {
        await _requestService.ConfigureRequestAuthorization(RequestId, auth);
        return NoContent();
    }

    [AjaxOnly]
    [HttpPost("{RequestId:int}/config/fixed-request")]
    [ValidateProjectRequest(RouteName.ProjectId, RouteName.RequestId)]
    public async Task<IActionResult> ConfigFixedRequest(int ProjectId, int RequestId, string[] fields, FixedRequestConfigViewModel config)
    {
        await _requestService.SaveFixedRequestConfig(RequestId, fields, config);
        return Ok(config);
    }

    [AjaxOnly]
    [HttpPost("{id:int}/delete")]
    [ValidateProjectRequest(RouteName.ProjectId, RouteName.RequestId)]
    public async Task<IActionResult> Delete(int RequestId)
    {
        await _requestService.Delete(RequestId);
        return Ok();
    }
}