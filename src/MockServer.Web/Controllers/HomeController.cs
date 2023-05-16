using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MockServer.Web.Models;

namespace MockServer.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        _logger.LogInformation(nameof(Index));
        return View();
    }
    public IActionResult Privacy()
    {
        _logger.LogInformation(nameof(Privacy));
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        return SignOut("Cookies", "oidc");
    }
}
