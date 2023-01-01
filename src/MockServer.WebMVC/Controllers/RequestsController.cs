using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MockServer.Core.Entities.Requests;
using MockServer.Core.Enums;
using MockServer.WebMVC.Attributes;
using MockServer.WebMVC.Models.Request;
using MockServer.WebMVC.Services.Interfaces;

namespace MockServer.WebMVC.Controllers;

[Authorize]
[Route("projects/{projectName}/requests")]
public class RequestsController : Controller
{
    private readonly IRequestService _requestService;
    public RequestsController(IRequestService requestService)
    {
        _requestService = requestService;
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(string projectName)
    {
        ViewData["ProjectName"] = projectName;
        return View("Views/Requests/Create.cshtml");
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(string projectName, CreateUpdateRequestModel request)
    {
        if (!ModelState.IsValid)
        {
            return View("Views/Requests/Create.cshtml", request);
        }
        int id = await _requestService.Create(projectName, request);
        return RedirectToAction(nameof(Config), new { projectName = projectName, id = id });
    }

    [AjaxOnly]
    [HttpPost("create")]
    public async Task<IActionResult> CreateAjax(string projectName, CreateUpdateRequestModel request)
    {
        if (!ModelState.IsValid)
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            return BadRequest(allErrors);
        }
        int id = await _requestService.Create(projectName, request);
        return Ok(new
        {
            id = id
        });
    }

    [HttpGet("{id:int}/config")]
    public async Task<IActionResult> Config(string projectName, int id)
    {
        var request = await _requestService.Get(projectName, id);
        Guard.Against.Null(request);
        ViewData["ProjectName"] = projectName;
        ViewData["RequestId"] = id;
        switch (request.Type)
        {
            case RequestType.Fixed:
                var vm = await _requestService.GetFixedRequestConfigViewModel(projectName, id);
                return View("Views/Requests/FixedRequestConfig.cshtml", vm);
            case RequestType.Forwarding:
                return View("Views/Requests/ForwardingRequestConfig.cshtml", id);
            case RequestType.Callback:
                return View("Views/Requests/CallbackRequestConfig.cshtml", id);
            default:
                return View("Views/Shared/Error.cshtml");
        }
    }

    [AjaxOnly]
    [HttpPost("{id:int}/config/fixed-request")]
    public async Task<IActionResult> ConfigFixedRequest(string projectName, int id, string[] fields, FixedRequestConfigViewModel config)
    {
        await _requestService.SaveFixedRequestConfig(projectName, id, fields, config);
        return Ok(config);
    }

    [AjaxOnly]
    [HttpPost("{id:int}/delete")]
    public async Task<IActionResult> Delete(string projectName, int id)
    {
        await _requestService.Delete(projectName, id);
        return Ok();
    }

    [AjaxOnly]
    [HttpGet("create")]
    public async Task<IActionResult> GetCreatePartial(string projectName)
    {
        ViewData["ProjectName"] = projectName;
        return PartialView("Views/Requests/_CreateRequestPartial.cshtml");
    }

    [AjaxOnly]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOpenParital(string projectName, int id)
    {
        ViewData["ProjectName"] = projectName;
        ViewData["RequestId"] = id;
        var vm = await _requestService.GetRequestOpenViewModel(projectName, id);
        return PartialView("Views/Projects/_RequestOpen.cshtml", vm);
    }
}