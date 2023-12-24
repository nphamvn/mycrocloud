using Microsoft.AspNetCore.Mvc;
using WebApp.Api.Controllers;
using WebApp.Domain.Repositories;

namespace WebApp.Api.Rest;

[Route("/api/apps/{appId:int}/logs")]
public class LogsController : BaseController
{
    private readonly ILogRepository _logRepository;

    public LogsController(ILogRepository logRepository)
    {
        _logRepository = logRepository;
    }
    public async Task<IActionResult> Search(int appId) {
        var logs = await _logRepository.Search(appId);
        return Ok(logs.Select(l => new {
            l.Id,
            Timestamp = l.CreatedAt,
            l.RouteId,
            l.Method,
            l.Path,
            l.StatusCode
        }));
    }
}
