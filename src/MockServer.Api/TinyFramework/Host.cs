using MockServer.Core.Models;
using MockServer.Core.Models.Projects;
namespace MockServer.Api.TinyFramework;

public class Host
{
    private readonly HttpContext _context;
    private readonly IServiceProvider _provider;

    public Host(IHttpContextAccessor contextAccessor, IServiceProvider provider)
    {
        _context = contextAccessor.HttpContext;
        _provider = provider;
    }

    public async Task Run() {
        var project = _context.Items["Project"] as Project;
        ArgumentNullException.ThrowIfNull(project);
        var builder = WebApplication.CreateBuilder(_provider);
        var app = builder.Build(project);
        app.Owner = new ApplicationUser
        {
            Id = project.UserId,
        };
        app.UseMiddleware<RoutingMiddleware>();
        if (project.UseMiddlewares.Contains(nameof(AuthenticationMiddleware)))
        {
            app.UseMiddleware<AuthenticationMiddleware>();
        }
        if (project.UseMiddlewares.Contains(nameof(ConstraintValidationMiddleware)))
        {
            app.UseMiddleware<ConstraintValidationMiddleware>();
        }
        await app.Handle(new Request
        {
            HttpContext = _context,
            WebApplication = app
        });
    }
}
