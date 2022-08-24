using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockServer.Data;
using MockServer.DTOs;
using MockServer.Entities;
using MockServer.Models.Request.Workspaces;
using MockServer.Models.Responses.Workspaces;

namespace MockServer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkspacesController : ControllerBase
{
    private readonly AppDbContext dbContext;

    public WorkspacesController(AppDbContext dbContext)
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
            .Select(ws => new GetWorkspaceResponseModel
            {
                Id = ws.Id,
                Name = ws.Name,
                FriendlyName = ws.FriendlyName,
                AccessScope = ws.AccessScope == 0 ? "Private" : "Public",
                ApiKey = ws.ApiKey
            })
            .ToList();
            return Ok(workspaces);
        }
        else
        {
            return NotFound("User not found");
        }
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetById(string name)
    {
        string username = "npham";
        var user = await dbContext.Users
                            .Include(u => u.Workspaces)
                            .Where(u => u.Username == username)
                            .FirstOrDefaultAsync();
        if (user != null)
        {
            var workspace = user.Workspaces
            .Where(ws => ws.Name == name)
            .Select(ws => new GetWorkspaceResponseModel
            {
                Id = ws.Id,
                Name = ws.Name,
                FriendlyName = ws.FriendlyName,
                AccessScope = ws.AccessScope == 0 ? "Private" : "Public",
                ApiKey = ws.ApiKey
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

    [HttpPost]
    public async Task<IActionResult> AddWorkspace([FromBody] AddWorkspaceRequestModel model)
    {
        string username = "npham";
        var user = await dbContext.Users
                            .Include(u => u.Workspaces)
                            .Where(u => u.Username == username)
                            .FirstOrDefaultAsync();
        if (user != null)
        {
            var workspace = user.Workspaces
            .Where(ws => ws.Name == model.Name)
            .FirstOrDefault();

            if (workspace == null)
            {
                user.Workspaces.Add(new Workspace
                {
                    Name = model.Name,
                    FriendlyName = model.FriendlyName,
                    AccessScope = model.AccessScope,
                    ApiKey = model.ApiKey
                });
                await dbContext.SaveChangesAsync();
                return Ok(workspace);
            }
            else
            {
                return NotFound("Workspace existing");
            }
        }
        else
        {
            return NotFound("User not found");
        }
    }

    [HttpDelete("{name}")]
    public async Task<IActionResult> DeleteWorkspace(string name)
    {
        string username = "npham";
        var user = await dbContext.Users
                            .Include(u => u.Workspaces)
                            .Where(u => u.Username == username)
                            .FirstOrDefaultAsync();
        if (user != null)
        {
            var workspace = user.Workspaces
            .Where(ws => ws.Name == name)
            .FirstOrDefault();

            if (workspace != null)
            {
                user.Workspaces.Remove(workspace);
                await dbContext.SaveChangesAsync();
                return Ok();
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

    //Workspace's request manage

    [HttpGet("{name}/requests")]
    public async Task<IActionResult> GetAllRequests(string name)
    {
        string username = "npham";
        var user = await dbContext.Users
                            .Include(u => u.Workspaces.Where(w => w.Name == name))
                            .ThenInclude(w => w.Requests)
                            .ThenInclude(r => r.Response)
                            .Where(u => u.Username == username)
                            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound("User not found");
        }
        var workspace = user.Workspaces.FirstOrDefault();
        if (workspace == null)
        {
            return NotFound("Workspace not found");
        }

        var requests = workspace.Requests
                .Select(req => new GetWorkspaceRequestModel
                {
                    Id = req.Id,
                    Name = req.Name,
                    Method = req.Method,
                    Path = req.Path,
                    Response = new GetWorkspaceRequestResponseModel
                    {
                        StatusCode = req.Response.StatusCode,
                        Body = req.Response.Body
                    }
                })
            .ToList();
        return Ok(new
        {
            workspace.Id,
            workspace.Name,
            workspace.FriendlyName,
            AccessScope = workspace.AccessScope == 0 ? "private" : "public",
            workspace.ApiKey,
            Requests = requests
        });
    }

    [HttpGet("{name}/requests/{id}")]
    public async Task<IActionResult> GetRequestById(string name, int id)
    {
        string username = "npham";
        var user = await dbContext.Users
                            .Include(u => u.Workspaces.Where(w => w.Name == name))
                            .ThenInclude(w => w.Requests.Where(r => r.Id == id))
                            .ThenInclude(r => r.Response)
                            .Where(u => u.Username == username)
                            .FirstOrDefaultAsync();
        if (user != null)
        {
            var workspace = user.Workspaces.FirstOrDefault();
            if (workspace == null)
            {
                return BadRequest("Workspace not found");
            }

            var request = workspace.Requests.FirstOrDefault();
            if (request == null)
            {
                return BadRequest("Request not found");
            }
            return Ok(new
            {
                request.Id,
                request.Name,
                request.Method,
                request.Path,
                Response = new
                {
                    request.Response.StatusCode,
                    request.Response.Body
                }
            });
        }
        else
        {
            return NotFound("User not found");
        }
    }

    [HttpPost("{name}/requests")]
    public async Task<IActionResult> AddItem(string name, [FromBody] AddRequestModel request)
    {
        string username = "npham";
        var user = await dbContext.Users
                            .Include(u => u.Workspaces.Where(w => w.Name == name))
                            .ThenInclude(w => w.Requests)
                            .Where(u => u.Username == username)
                            .FirstOrDefaultAsync();

        if (user != null)
        {
            var workspace = user.Workspaces.FirstOrDefault();
            if (workspace == null)
            {
                return BadRequest("Workspace not found");
            }

            var found = workspace.Requests
                            .Where(r => r.Method == request.Method && r.Path == request.Path)
                            .FirstOrDefault();
            if (found != null)
            {
                return BadRequest("Request existing");
            }
            else
            {
                var add = new Request
                {
                    Name = request.Name,
                    Method = request.Method,
                    Path = request.Path,
                    Response = new Response
                    {
                        StatusCode = request.Response.StatusCode,
                        Body = request.Response.Body
                    }
                };
                workspace.Requests.Add(add);
                await dbContext.SaveChangesAsync();
                return Ok(new
                {
                    add.Id,
                    add.Name,
                    add.Method,
                    add.Path,
                    Response = new
                    {
                        StatusCode = add.Response.StatusCode,
                        Body = add.Response.Body
                    }
                });
            }
        }
        else
        {
            return NotFound("User not found");
        }
    }

    [HttpDelete("{name}/requests/{id}")]
    public async Task<IActionResult> AddItem(string name, int id)
    {
        string username = "npham";
        var user = await dbContext.Users
                            .Include(u => u.Workspaces.Where(w => w.Name == name))
                            .ThenInclude(w => w.Requests.Where(r => r.Id == id))
                            .Where(u => u.Username == username)
                            .FirstOrDefaultAsync();
        if (user == null)
        {
            return NotFound("User not found");
        }
        var workspace = user.Workspaces.FirstOrDefault();
        if (workspace == null)
        {
            return BadRequest("Workspace not found");
        }

        var request = workspace.Requests.FirstOrDefault();
        if (request == null)
        {
            return BadRequest("Request not found");
        }
        workspace.Requests.Remove(request);
        await dbContext.SaveChangesAsync();
        return Ok();
    }
}