using Microsoft.AspNetCore.Mvc;

namespace WebApp.Api.Controllers;

public class TestController : BaseController
{
    [HttpGet("Ping")]
    public string Ping()
    {
        return "Pong";
    }
}