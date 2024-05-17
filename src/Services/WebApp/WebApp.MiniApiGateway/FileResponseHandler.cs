using WebApp.Infrastructure;
using Route = WebApp.Domain.Entities.Route;
namespace WebApp.MiniApiGateway;

public static class FileResponseHandler
{
    public static async Task Handle(HttpContext context)
    {
        var route = (Route)context.Items["_Route"]!;
        var dbContext = context.RequestServices.GetRequiredService<AppDbContext>();
        var file = await dbContext.Files.FindAsync(route.FileId);
        if (file is null)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("File not found");
            await context.Response.CompleteAsync();
            return;
        }
        foreach (var header in route.ResponseHeaders ?? [])
        {
            context.Response.Headers.Append(header.Name, header.Value);
        }
        await context.Response.Body.WriteAsync(file.Content);
        await context.Response.CompleteAsync();
    }
}