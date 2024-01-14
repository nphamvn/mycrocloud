using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Models;
using WebApp.Domain.Repositories;
using WebApp.Domain.Services;
using WebApp.RestApi.Extensions;

namespace WebApp.RestApi.Controllers;

public class AppsController(IAppService appService, IAppRepository appRepository) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] AppSearchRequest request)
    {
        var apps = await appRepository.ListByUserId(User.GetUserId(), "", "");
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
        await appService.Create(User.GetUserId(), appCreateRequest.ToEntity());
        return Created();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var app = await appRepository.GetByAppId(id);
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
        await appService.Rename(id, renameRequest.Name);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await appService.Delete(id);
        return NoContent();
    }
}