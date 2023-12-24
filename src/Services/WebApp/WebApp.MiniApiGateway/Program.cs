using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;
using WebApp.Infrastructure.Repositories.EfCore;
using Route = WebApp.Domain.Entities.Route;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});
builder.Services.AddScoped<IAppRepository, AppRepository>();
builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();


var app = builder.Build();

app.Use(async (context, next) => {
    var appRepository = context.RequestServices.GetService<IAppRepository>()!;
    var routeRepository = context.RequestServices.GetService<IRouteRepository>()!;
    var logRepository = context.RequestServices.GetService<ILogRepository>()!;
    
    int? appId = null;
    var fromHeader = true;
    if (fromHeader)
    {
        if (context.Request.Headers.TryGetValue("X-AppId", out var headerAppId))
        {
            appId = int.Parse(headerAppId.ToString()["App-".Length..]);
        }
    }

    if (appId is null)
    {
        context.Response.StatusCode = 404;
        return;
    }
    var app = await appRepository.GetByAppId(appId.Value);
    if (app is null)
    {
        context.Response.StatusCode = 404;
        return;
    }

    var appRoutes = await routeRepository.List(app.Id, "", "");
    var match = true;
    if (!match)
    {
        context.Response.StatusCode = 404;
        return;
    }
    var route = await routeRepository.GetById(appRoutes.First().Id);

    context.Items["_App"] = app;
    context.Items["_Route"] = route;

    await next(context);

    //TODO: Log
    await logRepository.Add(new Log {
        App = app,
        Route = route,
        Method = context.Request.Method,
        Path = context.Request.Path,
        StatusCode = context.Response.StatusCode
    });
});

app.Run(async context => {
    var app = (App)context.Items["_App"]!;
    var route = (Route)context.Items["_Route"]!;
    await context.Response.WriteAsJsonAsync(new {
        App = app,
        Route = route
    });
});

app.Run();

