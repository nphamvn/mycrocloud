using Microsoft.AspNetCore.Mvc;

namespace MycroCloud.WebMvc.Controllers;

public class HomeController(ILogger<HomeController> logger) : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }
}
