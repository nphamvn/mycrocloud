using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MockServer.Data;
using MockServer.DTOs;
using MockServer.Entities;
using MockServer.Models;

namespace MockServer.Services;

public class ActionResultService : IActionResultService
{
    private readonly AppDbContext dbContext;

    public ActionResultService(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<CustomActionResult> GetActionResult(RequestDto request)
    {
        var user = await dbContext.Users
                        .Include(u => u.Workspaces.Where(w => w.Name == request.Workspace))
                        .ThenInclude(w => w.Requests.Where(req => req.Method == request.Method && req.Path == request.Path))
                        .ThenInclude(r => r.Response)
                        .Where(u => u.Username == request.Username)
                        .FirstOrDefaultAsync();

        Response response = null;
        if (user != null)
        {
            var workspace = user.Workspaces.FirstOrDefault();
            if (workspace != null)
            {
                if (workspace.AccessScope == 0)
                {
                    if (request.ApiKey != workspace.ApiKey)
                    {
                        response = new Response
                        {
                            StatusCode = 401,
                            Body = "You are not allowed to access this api"
                        };
                        return new CustomActionResult(response);
                    }
                }
                var req = workspace.Requests.FirstOrDefault();
                if (req != null)
                {
                    response = req.Response;
                }
            }
        }
        return new CustomActionResult(response);
    }
}