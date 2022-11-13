using Microsoft.AspNetCore.Mvc;

namespace MockServer.WebMVC.Controllers;

[Route("projects")]
public class ProjectsController : Controller
{
    public async Task<IActionResult> Index()
    {
        return Ok(nameof(Index));
    }

    [Route("{name}")]
    public async Task<IActionResult> Detail(string name)
    {
        return Ok(nameof(Detail) + ": " + name);
    }
}