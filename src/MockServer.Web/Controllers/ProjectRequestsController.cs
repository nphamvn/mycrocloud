using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MockServer.Web.Attributes;
using MockServer.Web.Filters;
using MockServer.Web.Models.Common;
using MockServer.Web.Models.ProjectRequests;
using MockServer.Web.Services.Interfaces;
using RouteName = MockServer.Web.Common.Constants.RouteName;
namespace MockServer.Web.Controllers;

[Authorize]
[Route("projects/{ProjectName}/requests")]
[GetAuthUserProjectId(RouteName.ProjectName, RouteName.ProjectId)]
public class ProjectRequestsController : Controller
{
    private readonly IProjectRequestWebService _requestService;
    public ProjectRequestsController(IProjectRequestWebService requestService)
    {
        _requestService = requestService;
    }

    [HttpGet("/projects/{ProjectName}")]
    [GetAuthUserProjectId(RouteName.ProjectName, RouteName.ProjectId)]
    public async Task<IActionResult> Index(int ProjectId)
    {
        var vm = await _requestService.GetIndexViewModel(ProjectId);
        return View("Views/ProjectRequests/Index.cshtml", vm);
    }

    [AjaxOnly]
    [HttpGet("create")]
    public async Task<IActionResult> GetCreatePartial(int ProjectId)
    {
        var model = await _requestService.GetCreateRequestViewModel(ProjectId);
        return PartialView("Views/ProjectRequests/_CreateRequestPartial.cshtml", model);
    }

    [AjaxOnly]
    [HttpPost("create")]
    public async Task<IActionResult> CreateAjax(int ProjectId, SaveRequestViewModel request)
    {
        if (await _requestService.ValidateCreate(ProjectId, request, ModelState))
        {
            return Ok(new AjaxResult<SaveRequestViewModel>
            {
                Errors = new List<Error>{ new("something went wrong") }
            });
        }
        int id = await _requestService.Create(ProjectId, request);
        return Ok(new AjaxResult());
    }

    [AjaxOnly]
    [HttpGet("{RequestId:int}/edit")]
    [ValidateProjectRequest(RouteName.ProjectId, RouteName.RequestId)]
    public async Task<IActionResult> GetEditPartial(int ProjectId, int RequestId)
    {
        var vm = await _requestService.GetEditRequestViewModel(RequestId);
        ViewData["FormMode"] = "Edit";
        return PartialView("Views/Requests/_CreateRequestPartial.cshtml", vm);
    }

    [AjaxOnly]
    [HttpPost("{RequestId:int}/edit")]
    [ValidateProjectRequest(RouteName.ProjectId, RouteName.RequestId)]
    public async Task<IActionResult> Edit(int RequestId, SaveRequestViewModel request)
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
    [HttpGet("{RequestId:int}")]
    [ValidateProjectRequest(RouteName.ProjectId, RouteName.RequestId)]
    public async Task<IActionResult> View(int RequestId)
    {
        var vm = await _requestService.GetRequestOpenViewModel(RequestId);
        return PartialView("Views/Requests/_RequestOpen.cshtml", vm);
    }

    [AjaxOnly]
    [HttpGet("{RequestId:int}/authorization")]
    [ValidateProjectRequest(RouteName.ProjectId, RouteName.RequestId)]
    public async Task<IActionResult> GetAuthorizationPartial(int ProjectId, int id)
    {
        var vm = await _requestService.GetAuthorization(ProjectId, id);
        return PartialView("Views/Requests/_AuthorizationConfigPartial.cshtml", vm);
    }

    [AjaxOnly]
    [HttpPost("{RequestId:int}/authorization")]
    [ValidateProjectRequest(RouteName.ProjectId, RouteName.RequestId)]
    public async Task<IActionResult> ConfigAuthorization(int ProjectId, int RequestId, AuthorizationConfiguration auth)
    {
        await _requestService.AttachAuthorization(RequestId, auth);
        return NoContent();
    }

    [AjaxOnly]
    [HttpPost("{RequestId:int}/request")]
    [ValidateProjectRequest(RouteName.ProjectId, RouteName.RequestId)]
    public async Task<IActionResult> SaveRequestConfiguration(int RequestId, RequestConfiguration config)
    {
        await _requestService.SaveRequestConfiguration(RequestId, config);
        return Ok(new AjaxResult<RequestConfiguration>
        {
            Data = config
        });
    }

    [AjaxOnly]
    [HttpPost("{RequestId:int}/delete")]
    [ValidateProjectRequest(RouteName.ProjectId, RouteName.RequestId)]
    public async Task<IActionResult> Delete(int RequestId)
    {
        await _requestService.Delete(RequestId);
        return Ok();
    }
}