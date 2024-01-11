using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Controllers;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Repositories.EfCore;

namespace WebApp.Api.Rest.Controllers;

public class DatabaseServersController(AppDbContext dbContext) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var servers = await dbContext.Servers.Where(s => s.UserId == User.GetUserId()).ToListAsync();
        return Ok(servers.Select(s => new
        {
            s.Id,
            s.Name,
            s.CreatedAt,
            s.UpdatedAt
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Create(Server server)
    {
        server.UserId = User.GetUserId();
        server.CreatedAt = DateTime.UtcNow;
        await dbContext.Servers.AddAsync(server);
        await dbContext.SaveChangesAsync();
        return Created();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var server = await dbContext.Servers.Include(server => server.Databases).SingleAsync(s => s.Id == id);
        return Ok(new
        {
            server.Id,
            server.Name,
            server.LoginId,
            server.CreatedAt,
            server.UpdatedAt,
            DatabaseCount = server.Databases?.Count ?? 0
        });
    }
    
    [HttpGet("{id:int}/Databases")]
    public async Task<IActionResult> GetDatabases(int id)
    {
        var dbs = await dbContext.Databases.Where(db => db.ServerId == id).ToListAsync();
        return Ok(dbs.Select(db => new
        {
            db.Id,
            db.Name,
            db.CreatedAt,
            db.UpdatedAt
        }));
    }
    
    [HttpPost("{id:int}/Databases")]
    public async Task<IActionResult> CreateDatabase(int id, Database database)
    {
        var server = await dbContext.Servers.SingleAsync(s => s.Id == id);
        database.Server = server;
        await dbContext.Databases.AddAsync(database);
        await dbContext.SaveChangesAsync();
        return Created();
    }
}