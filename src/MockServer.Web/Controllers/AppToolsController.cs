using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MockServer.Web.Controllers;

public class AppToolsController : BaseController
{
    [AllowAnonymous]
    [Route("/tools/request-tester")]
    public IActionResult Open(string url, string method)
    {
        ViewData["Url"] = url ?? "http://localhost:5000/dev/render-expression-template";
        ViewData["Method"] = method;
        return View("/Views/AppTools/HttpRequestSender.cshtml");
    }

    [AllowAnonymous]
    [Route("/ping")]
    public IActionResult Ping()
    {
        return Ok(new
        {
            Message = "pong"
        });
    }

    [AllowAnonymous]
    [Route("/me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            Request = HttpContextExtentions.GetRequestDictionary(HttpContext)
        });
    }
    
    [AllowAnonymous]
    [Route("/dic")]
    public IActionResult Dic()
    {
        var dic = new Dictionary<string, string>
        {
            {"key1", "value1"},
            {"key2", "value2"},
        };
        return Ok(dic);
    }
    [AllowAnonymous]
    [Route("/list")]
    public IActionResult List()
    {
        var list = new List<string>{
            "one",
            "two"
        };
        return Ok(list);
    }
}
