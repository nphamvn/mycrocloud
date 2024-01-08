using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Models;
using WebApp.Api.Rest;
using WebApp.Domain.Services;
using WebApp.Domain.Repositories;

namespace WebApp.Api.Controllers;

public class AppsController : BaseController
{
    private readonly IAppService _appService;
    private readonly IAppRepository _appRepository;

    public AppsController(IAppService appService, IAppRepository appRepository)
    {
        _appService = appService;
        _appRepository = appRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] AppSearchRequest request)
    {
        var apps = await _appRepository.ListByUserId(User.GetUserId(), "", "");
        return Ok(apps.Select(a => new {
            a.Id,
            a.Name,
            a.Description,
            a.CreatedAt,
            a.UpdatedAt
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Create(AppCreateRequest appCreateRequest)
    {
        await _appService.Create(User.GetUserId(), appCreateRequest.ToEntity());
        return Created();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var app = await _appRepository.GetByAppId(id);
        return Ok(new {
            app.Id,
            app.Name,
            app.Description,
            app.CreatedAt,
            app.UpdatedAt
        });
    }

    [HttpPatch("{id:int}/Rename")]
    public async Task<IActionResult> Rename(int id, AppRenameRequest renameRequest)
    {
        await _appService.Rename(id, renameRequest.Name);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _appService.Delete(id);
        return NoContent();
    }
}