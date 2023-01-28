using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MockServer.Web.Controllers;

[Route("[controller]")]
public class ExploreController : BaseController
{
    private readonly ILogger<ExploreController> _logger;

    public ExploreController(ILogger<ExploreController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
