using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Models;
using WebApp.Domain.Entities;
using WebApp.Domain.Enums;
using WebApp.Domain.Repositories;
using WebApp.Domain.Services;
using WebApp.RestApi.Extensions;

namespace WebApp.RestApi.Controllers;

public class AppsController(IAppService appService, IAppRepository appRepository) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index(string? term)
    {
        var apps = await appRepository.ListByUserId(User.GetUserId(), term, "");
        return Ok(apps.Select(app => new {
            app.Id,
            app.Name,
            app.Description,
            Status = app.Status.ToString(),
            app.CreatedAt,
            app.UpdatedAt
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Create(AppCreateRequest appCreateRequest)
    {
        var app = appCreateRequest.ToEntity();
        await appService.Create(User.GetUserId(), app);
        return Created(app.Id.ToString(), app);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var app = await appRepository.GetByAppId(id);
        return Ok(new {
            app.Id,
            app.Name,
            app.Description,
            Status = app.Status.ToString(),
            app.CreatedAt,
            app.UpdatedAt
        });
    }

    [HttpPatch("{id:int}/Rename")]
    public async Task<IActionResult> Rename(int id, AppRenameRequest renameRequest)
    {
        await appService.Rename(id, renameRequest.Name);
        return NoContent();
    }

    [HttpPatch("{id:int}/Status")]
    public async Task<IActionResult> SetStatus(int id, AppStatus status)
    {
        await appService.SetStatus(id, status);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await appService.Delete(id);
        return NoContent();
    }

    [HttpPatch("{id:int}/cors")]
    public async Task<IActionResult> Cors(int id, CorsSettings settings)
    {
        await appService.SetCorsSettings(id, settings);
        return NoContent();
    }

    [HttpGet("{id:int}/cors")]
    public async Task<IActionResult> Cors(int id)
    {
        var app = await appRepository.GetByAppId(id);
        return Ok(app.CorsSettings);
    }
}