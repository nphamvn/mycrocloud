using MockServer.Api.TinyFramework;
using MockServer.Core.Models;
using MockServer.Core.Models.Projects;
using WebApplication = MockServer.Api.TinyFramework.WebApplication;
namespace MockServer.Api.Services;

public class Container
{
    private readonly HttpContext _context;
    private readonly IServiceProvider _provider;

    public Container(IHttpContextAccessor contextAccessor, IServiceProvider provider)
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
