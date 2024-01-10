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
    public async Task<IActionResult> Search(int appId, [FromQuery]List<int>? routeIds, DateTime? accessDateFrom,
     DateTime? accessDateTo, int page = 1, int pageSize = 50, string? sort = null) {
        var logs = await _logRepository.Search(appId);
        if (routeIds?.Count > 0) logs = logs.Where(l => l.RouteId != null && routeIds.Contains(l.RouteId.Value));
        if (accessDateFrom is not null) logs = logs.Where(l => l.CreatedAt.Date >= accessDateFrom.Value.Date);
        if (accessDateTo is not null) logs = logs.Where(l => l.CreatedAt.Date <= accessDateTo.Value.Date);
        if (!string.IsNullOrEmpty(sort))
        {
            
        }
        logs = logs
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        return Ok(logs.Select(l => new {
            l.Id,
            Timestamp = l.CreatedAt,
            l.RouteId,
            l.Method,
            l.Path,
            l.StatusCode,
            l.FunctionExecutionDuration,
            l.AdditionalLogMessage
        }));
    }
}
