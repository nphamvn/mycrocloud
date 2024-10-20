using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Infrastructure;
using WebApp.RestApi.Filters;

namespace WebApp.RestApi.Controllers;

[Route("apps/{appId:int}/[controller]")]
[TypeFilter<AppOwnerActionFilter>(Arguments = ["appId"])]
public class ObjectStorageController(AppDbContext appDbContext) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> ListObjects(int appId, string? prefix = null)
    {
        var app = await appDbContext.Apps
                .Include(app => app.Objects)
                .SingleAsync(app => app.Id == appId)
            ;

        return Ok(app.Objects.Select(obj => new
        {
            obj.Key,
            obj.CreatedAt,
            obj.UpdatedAt
        }));
    }
    
    [HttpGet("{key}")]
    public async Task<IActionResult> GetObject(int appId, string key)
    {
        var app = await appDbContext.Apps
                .Include(app => app.Objects)
                .SingleAsync(app => app.Id == appId)
            ;
        
        var obj = app.Objects.SingleOrDefault(obj => obj.Key == key);

        if (obj is null)
        {
            return NotFound();
        }

        return File(obj.Content, "application/octet-stream");
    }
}