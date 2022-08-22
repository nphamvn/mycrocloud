using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockServer.Data;
using MockServer.DTOs;
using MockServer.Models;

namespace MockServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkspaceController : ControllerBase
{
    private readonly AppDbContext dbContext;

    public WorkspaceController(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        string username = "npham";
        var user = await dbContext.Users
                            .Include(u => u.Workspaces)
                            .Where(u => u.Username == username)
                            .FirstOrDefaultAsync();
        if (user != null)
        {
            var workspaces = user.Workspaces
            .Select(ws => new
            {
                ws.Id,
                ws.Name
            })
            .ToList();
            return Ok(workspaces);
        }
        else
        {
            return NotFound("User not found");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        Console.WriteLine("Id: " + id);
        string username = "npham";
        var user = await dbContext.Users
                            .Include(u => u.Workspaces)
                            .Where(u => u.Username == username)
                            .FirstOrDefaultAsync();
        if (user != null)
        {
            var workspace = user.Workspaces
            .Where(ws => ws.Id == id)
            .Select(ws => new
            {
                ws.Id,
                ws.Name
            })
            .FirstOrDefault();

            if (workspace != null)
            {
                return Ok(workspace);
            }
            else
            {
                return NotFound("Workspace not found");
            }
        }
        else
        {
            return NotFound("User not found");
        }
    }
    [HttpGet("{id}/items")]
    public async Task<IActionResult> GetAllRequests(int id)
    {
        Console.WriteLine("Id: " + id);
        string username = "npham";
        var user = await dbContext.Users
                            .Include(u => u.Workspaces.Where(w => w.Id == id))
                            .ThenInclude(w => w.Requests)
                            .ThenInclude(r => r.Response)
                            .Where(u => u.Username == username)
                            .FirstOrDefaultAsync();
        if (user != null)
        {
            var workspace = user.Workspaces
            .Select(ws => new
            {
                ws.Id,
                ws.Name,
                Requests = ws.Requests
                .Select(req => new
                {
                    req.Id,
                    req.Name,
                    req.Method,
                    req.Path,
                    Response = new
                    {
                        req.Response.StatusCode,
                        req.Response.Body
                    }
                })
                .ToList()
            })
            .FirstOrDefault();

            if (workspace != null)
            {
                return Ok(workspace);
            }
            else
            {
                return NotFound("Workspace not found");
            }
        }
        else
        {
            return NotFound("User not found");
        }
    }

    [HttpPost("{id}/items")]
    public async Task<IActionResult> AddItem(int id, [FromBody] ItemDto item)
    {
        Console.WriteLine("Id: " + id);
        string username = "npham";
        var user = await dbContext.Users
                            .Include(u => u.Workspaces.Where(w => w.Id == id))
                            .ThenInclude(w => w.Requests)
                            .ThenInclude(r => r.Response)
                            .Where(u => u.Username == username)
                            .FirstOrDefaultAsync();

        if (user != null)
        {
            var workspace = user.Workspaces.FirstOrDefault();

            var request = workspace.Requests
                            .Where(r => r.Method == item.Request.Method && r.Path == item.Request.Path)
                            .FirstOrDefault();
            if (request != null)
            {
                return BadRequest("Request existing");
            }
            else
            {
                var addRequest = new Request();
                addRequest.Name = item.Request.Name;
                addRequest.Method = item.Request.Method;
                addRequest.Path = item.Request.Path;
                addRequest.Response = new Response
                {
                    StatusCode = item.Response.StatusCode,
                    Body = item.Response.Body
                };
                workspace.Requests.Add(addRequest);
                dbContext.SaveChanges();
                return Ok();
            }
        }
        else
        {
            return NotFound("User not found");
        }
    }
}