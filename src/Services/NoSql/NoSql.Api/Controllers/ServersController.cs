using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoSql.Api.Extensions;
using NoSql.Core.Data;
using NoSql.Core.Entities;

namespace NoSql.Api.Controllers;

public class ServersController(AppDbContext dbContext) : BaseController
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
}