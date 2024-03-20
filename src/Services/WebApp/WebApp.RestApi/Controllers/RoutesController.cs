using Microsoft.AspNetCore.Mvc;
using WebApp.Domain.Enums;
using WebApp.Domain.Repositories;
using WebApp.Domain.Services;
using WebApp.RestApi.Models.Routes;

namespace WebApp.RestApi.Controllers;

[Route("apps/{appId:int}/[controller]")]
public class RoutesController(IRouteService routeService,
    IRouteRepository routeRepository
    ) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Index(int appId, string? SearchTerm, string? Sort)
    {
        var routes = await routeRepository.List(appId, SearchTerm, Sort);
        return Ok(routes.Select(route => new {
            route.Id,
            route.Name,
            route.Method,
            route.Path,
            Status = route.Status.ToString(),
            route.CreatedAt,
            route.UpdatedAt
        }));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var route = await routeRepository.GetById(id);
        return Ok(new {
            route.Id,
            route.AppId,
            route.Name,
            route.Method,
            route.Path,
            route.RequestQuerySchema,
            route.RequestHeaderSchema,
            route.RequestBodySchema,
            route.ResponseType,
            route.ResponseStatusCode,
            ResponseHeaders = (route.ResponseHeaders ?? []).Select(h => new {
                h.Name,
                h.Value
            }),
            route.UseDynamicResponse,
            route.ResponseBody,
            route.ResponseBodyLanguage,
            route.FunctionHandler,
            route.FunctionHandlerDependencies,
            route.RequireAuthorization,
            Status = route.Status.ToString(),
            route.FileId,
            FileName = route.File?.Name,
            FileFolderId = route.File?.FolderId,
            route.CreatedAt,
            route.UpdatedAt
        });
    }

    [HttpPost("{id:int}/Clone")]
    public async Task<IActionResult> Clone(int id)
    {
        var newRouteId = await routeService.Clone(id);
        var newRoute = await routeRepository.GetById(newRouteId);
        return Created("", newRoute);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int appId, RouteCreateUpdateRequest route)
    {
        var entity = route.ToCreateEntity();
        await routeService.Create(appId, entity);
        return Created("", entity);
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Edit(int id, RouteCreateUpdateRequest route)
    {
        var currentRoute = await routeRepository.GetById(id);
        if (currentRoute.Status == RouteStatus.Blocked)
        {
            // Do not allow editing blocked route
            return BadRequest();
        }
        route.ToUpdateEntity(currentRoute);
        await routeService.Update(id, currentRoute);
        return NoContent();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var route = await routeRepository.GetById(id);
        if (route.Status == RouteStatus.Blocked)
        {
            // Do not allow deleting blocked route
            return BadRequest();
        }
        await routeService.Delete(id);
        return NoContent();
    }
}