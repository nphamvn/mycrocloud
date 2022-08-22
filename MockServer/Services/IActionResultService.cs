using MockServer.DTOs;
using MockServer.Models;

namespace MockServer.Services;

public interface IActionResultService
{
    Task<CustomActionResult> GetActionResult(RequestDto request);
}