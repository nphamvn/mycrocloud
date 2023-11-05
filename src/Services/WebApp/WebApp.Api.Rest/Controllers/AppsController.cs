using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Models;
using WebApp.Api.Services;

namespace WebApp.Api.Controllers;

public class AppsController : BaseController
{
    private readonly IAppService _appService;

    public AppsController(IAppService appService)
    {
        _appService = appService;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery]AppSearchRequest request)
    {
        return Ok(request);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create(AppCreateRequest appCreateRequest)
    {
        await _appService.Create(appCreateRequest);
        return Ok(appCreateRequest);
    }

    [HttpGet("{appId:int}/Details")]
    public async Task<IActionResult> Details(int appId)
    {
        return Ok(appId);
    }
    
    [HttpPost("{appId:int}/Rename")]
    public async Task<IActionResult> Rename(int appId, string newName)
    {
        return RedirectToAction(nameof(Index), new { WebApplicationName = newName });
    }

    [HttpPost("{appId:int}/Delete")]
    public async Task<IActionResult> Delete(int appId)
    {
        return NoContent();
    }
}