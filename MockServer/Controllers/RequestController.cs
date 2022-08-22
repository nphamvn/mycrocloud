using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MockServer.Data;
using MockServer.DTOs;
using MockServer.Models;
using MockServer.Services;

namespace MockServer.Controllers;

[ApiController]
public class RequestController : ControllerBase
{
    private readonly IActionResultService actionResultService;
    private readonly AppDbContext dbContext;

    public RequestController(IActionResultService actionResultService, AppDbContext dbContext)
    {
        this.dbContext = dbContext;
        this.actionResultService = actionResultService;
    }

    [Route("{username}/api/{workspace}/{*path}")]
    public async Task<IActionResult> Handle(string username, string workspace, string path)
    {
        RequestDto request = new RequestDto();
        request.Username = username;
        request.Workspace = workspace;
        request.Path = path;
        request.Method = HttpContext.Request.Method;
        CustomActionResult result = await actionResultService.GetActionResult(request);
        return result;
    }

    [Route("api/{username}/workspace")]
    public async Task<IActionResult> GetWorkspace(string username)
    {
        var workspaces = await dbContext.Users
                            .Include(u => u.Workspaces)
                            .Where(u => u.Username == username)
                            .ToListAsync();
        return Ok(workspaces);
    }
}