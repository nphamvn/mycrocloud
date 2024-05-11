using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Domain.Enums;
using WebApp.Domain.Repositories;
using WebApp.Domain.Services;
using WebApp.Infrastructure.Repositories.EfCore;
using WebApp.RestApi.Filters;
using WebApp.RestApi.Models.Routes;

namespace WebApp.RestApi.Controllers;

[Route("apps/{appId:int}/[controller]")]
[TypeFilter<AppOwnerActionFilter>(Arguments = ["appId"])]
public class RoutesController(IRouteService routeService,
    IRouteRepository routeRepository,
    AppDbContext appDbContext
    ) : BaseController
{
    private App App => (HttpContext.Items["App"] as App)!;
    
    [HttpGet]
    public async Task<IActionResult> Index(int appId, string? q, string? s)
    {
        var routes = await routeRepository.List(appId, q, s);
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
    
    [HttpGet("v2")]
    public async Task<IActionResult> IndexV2(int appId, string? q, string? s)
    {
        var folders = appDbContext.RouteFolders
            .Where(f => f.App.Id == appId)
            .Select(f => new IndexItemV2
            {
                Type = RouteRouteFolderType.Folder,
                Id = f.Id,
                ParentId = f.Parent != null ? f.Parent.Id : null,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt,
                RouteName = null,
                RouteMethod = null,
                RoutePath = null,
                RouteStatus = null,
                FolderName = f.Name
            });
        
        var routes = appDbContext.Routes
            .Where(r => r.App.Id == appId)
            .Select(r => new IndexItemV2
            {
                Type = RouteRouteFolderType.Route,
                Id = r.Id,
                ParentId = r.Folder != null ? r.Folder.Id : null,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt,
                RouteName = r.Name,
                RouteMethod = r.Method,
                RoutePath = r.Path,
                RouteStatus = r.Status,
                FolderName = null
            });

        var items = await folders.Union(routes).ToListAsync();
        
        return Ok(items.Select(item => new
        {
            Type = item.Type.ToString(),
            item.Id,
            item.ParentId,
            Route = item.Type == RouteRouteFolderType.Route
                ? new
                {
                    Name = item.RouteName,
                    Method = item.RouteMethod,
                    Path = item.RoutePath,
                    Status = item.RouteStatus.ToString()
                }
                : null,
            Folder = item.Type == RouteRouteFolderType.Folder
                ? new
                {
                    Name = item.FolderName
                }
                : null
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

        entity.Folder = route.FolderId != null
            ? await appDbContext.RouteFolders.SingleAsync(f =>
                f.App.Id == appId && f.Id == route.FolderId.Value)
            : null;
        
        await routeService.Create(appId, entity);
        return Created("", new { entity.Id, entity.Version });
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
    
    [HttpPost("folders")] 
    public async Task<IActionResult> CreateFolder(int appId, FolderCreateRequest folderCreateRequest)
    {
        var app = await appDbContext.Apps.SingleAsync(a => a.Id == appId);
        
        var parentFolder = folderCreateRequest.ParentId != null
            ? await appDbContext.RouteFolders.SingleAsync(f =>
                f.App == app && f.Id == folderCreateRequest.ParentId.Value)
            : null;
        
        var newFolder = new RouteFolder
        {
            App = app,
            Name = folderCreateRequest.Name,
            Parent = parentFolder,
            Version = Guid.NewGuid()
        };
        
        await appDbContext.RouteFolders.AddAsync(newFolder);

        await appDbContext.SaveChangesAsync();
        
        return Created(newFolder.Id.ToString(), new
        {
            newFolder.Id,
            newFolder.Name,
            newFolder.Version
        });
    }
    
    [HttpDelete("folders/{id:int}")]
    public async Task<IActionResult> DeleteFolder(int appId, int id)
    {
        var folder = await appDbContext.RouteFolders.SingleAsync(f => f.App == App && f.Id == id);
        
        const string sql = 
"""
CREATE TEMP TABLE folders_to_delete AS (
   WITH RECURSIVE cte AS (
       SELECT "Id" FROM "RouteFolders" WHERE "Id" = @id
       UNION ALL
       SELECT rf."Id" FROM "RouteFolders" rf JOIN cte ON rf."ParentId" = cte."Id"
   )
   SELECT "Id" FROM cte
);

DELETE FROM "Routes" WHERE "FolderId" IN (SELECT "Id" FROM folders_to_delete);
DELETE FROM "RouteFolders" WHERE "Id" IN (SELECT "Id" FROM folders_to_delete);

""";
        
        await appDbContext.Database.GetDbConnection().ExecuteAsync(sql, new { id = folder.Id });
        
        return NoContent();
    }
}