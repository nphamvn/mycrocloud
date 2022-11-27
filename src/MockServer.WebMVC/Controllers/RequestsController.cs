using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> Create(string projectName, CreateRequestViewModel request)
    {
        await _requestService.Create(projectName, request);
        return RedirectToAction("Detail", "Projects", new { name = projectName });
    }
}