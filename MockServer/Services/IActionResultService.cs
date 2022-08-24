using MockServer.DTOs;
using MockServer.Models;

namespace MockServer.Services;

public interface IActionResultService
{
    //Task<bool> Auth();
    Task<CustomActionResult> GetActionResult(RequestDto request);
}