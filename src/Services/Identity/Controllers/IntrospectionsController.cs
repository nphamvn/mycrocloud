using Identity.Models;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers;

[Route("Introspections")]
public class IntrospectionsController : ControllerBase
{
    [HttpPost("TokenIntrospect")]
    public async Task<IActionResult> TokenIntrospect(TokenIntrospectionRequest tokenIntrospectionRequest)
    {
        Console.WriteLine("OK");
        return Ok(tokenIntrospectionRequest);
    }
}