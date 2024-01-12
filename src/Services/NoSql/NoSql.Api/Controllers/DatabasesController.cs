using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoSql.Core.Data;
using NoSql.Core.Entities;

namespace NoSql.Api.Controllers;

public class DatabasesController(AppDbContext dbContext) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> List(int? serverId)
    {
        var dbs = dbContext.Databases.AsQueryable();
        if (serverId is not null)
        {
            dbs = dbs.Where(db => db.ServerId == serverId.Value);
        }
        return Ok(dbs.Select(db => new
        {
            db.Id,
            db.Name,
            db.CreatedAt,
            db.UpdatedAt
        }));
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(int serverId, Database database)
    {
        var server = await dbContext.Servers.SingleAsync(s => s.Id == serverId);
        database.Server = server;
        await dbContext.Databases.AddAsync(database);
        await dbContext.SaveChangesAsync();
        return Created();
    }
}