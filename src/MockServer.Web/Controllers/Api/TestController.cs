using Microsoft.AspNetCore.Mvc;
using MockServer.Web.Models.Common;
using MockServer.Web.Services.Interfaces;

namespace MockServer.Web.Controllers.Api;

public class TestController : ApiController
{
    [HttpGet("ok")]
    public IActionResult GetOk()
    {
        return Ok(new AjaxResult<Foo>
        {
            Data = new Foo { Bar = 1312 }
        });
    }
    [HttpGet("error")]
    public IActionResult GetErro()
    {
        AjaxResult result = new();
        result.AddError("something went wrong");
        return BadRequest(result);
    }
}
public class Foo
{
    public int Bar { get; set; }
}