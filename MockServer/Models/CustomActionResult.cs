using Microsoft.AspNetCore.Mvc;

namespace MockServer.Models;

public class CustomActionResult : IActionResult
{
    private readonly Response response;

    public CustomActionResult(Response response)
    {
        this.response = response;
    }
    public async Task ExecuteResultAsync(ActionContext context)
    {
        if (response == null)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.HttpContext.Response.WriteAsync("Request not found");
            return;
        }
        context.HttpContext.Response.StatusCode = response.StatusCode;
        if (!string.IsNullOrEmpty(response.Body))
        {
            await context.HttpContext.Response.WriteAsync(response.Body);
            return;
        }
        await context.HttpContext.Response.CompleteAsync();
    }
}