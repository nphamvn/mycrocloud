using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MockServer.WebMVC.Controllers;

public class AppToolsController : BaseController
{
    [AllowAnonymous]
    [Route("/tools/request-tester")]
    public IActionResult Open(string url, string method)
    {
        ViewData["Url"] = url;
        ViewData["Method"] = method;
        return View("Views/AppTools/HttpRequestSender.cshtml");
    }
}
