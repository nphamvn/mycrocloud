using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Models;
using WebApp.Domain.Repositories;
using WebApp.Domain.Services;

namespace WebApp.Api.Controllers;

[Route("api/apps/{appId:int}/routes")]
public class RoutesController : BaseController
{
    private readonly IRouteService _routeService;
    private readonly IRouteRepository _routeRepository;

    public RoutesController(IRouteService routeService, IRouteRepository routeRepository)
    {
        _routeService = routeService;
        _routeRepository = routeRepository;
    }
    [HttpGet]
    public async Task<IActionResult> Index(int appId, string? SearchTerm, string? Sort)
    {
        var routes = await _routeRepository.List(appId, SearchTerm, Sort);
        return Ok(routes.Select(r => new {
            r.Id,
            r.Name,
            r.Method,
            r.Path,
            r.Description,
            r.ResponseText,
            r.CreatedAt,
            r.UpdatedAt
        }));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var route = await _routeRepository.GetById(id);
        return Ok(new {
            route.Id,
            route.AppId,
            route.Name,
            route.Method,
            route.Path,
            route.ResponseText
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(int appId, RouteCreateUpdateRequest route)
    {
        return Ok(await _routeService.Create(appId, route.ToEntity()));
    }
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Edit(int id, RouteCreateUpdateRequest route)
    {
        var currentRoute = await _routeRepository.GetById(id);
        route.ToEntity(currentRoute);
        await _routeService.Update(id, currentRoute);
        return NoContent();
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return Ok();
    }
}