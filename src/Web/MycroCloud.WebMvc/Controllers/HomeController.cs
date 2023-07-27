using Microsoft.AspNetCore.Mvc;

namespace MycroCloud.WebMvc.Controllers;

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
}
