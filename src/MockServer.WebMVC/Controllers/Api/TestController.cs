using Microsoft.AspNetCore.Mvc;
using MockServer.WebMVC.Models.Common;
using MockServer.WebMVC.Services.Interfaces;

namespace MockServer.WebMVC.Controllers.Api;

public class TestController : ApiController
{
    [HttpGet("ok")]
    public IActionResult GetOk() {
        return Ok(new AjaxResult<Foo> {
            Data = new Foo {Bar = 1312}
        });
    }
    [HttpGet("error")]
    public IActionResult GetErro() {
        AjaxResult result = new();
        result.AddError("something went wrong");
        return BadRequest(result);
    }
}
public class Foo {
    public int Bar { get; set; }
}