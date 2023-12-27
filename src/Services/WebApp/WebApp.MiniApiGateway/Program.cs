using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;
using WebApp.Infrastructure.Repositories.EfCore;
using Route = WebApp.Domain.Entities.Route;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(options =>
{
    options.AddSeq(builder.Configuration["Logging:Seq:ServerUrl"]);
});
builder.Services.AddHttpLogging(o => { });
builder.Services.AddDbContext<AppDbContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});
builder.Services.AddScoped<IAppRepository, AppRepository>();
builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();


var app = builder.Build();
app.UseHttpLogging();
app.Use(async (context, next) => {
    var appRepository = context.RequestServices.GetService<IAppRepository>()!;
    var routeRepository = context.RequestServices.GetService<IRouteRepository>()!;
    var logRepository = context.RequestServices.GetService<ILogRepository>()!;
    
    int? appId = null;
    var fromHeader = true;
    if (fromHeader)
    {
        if (context.Request.Headers.TryGetValue("X-AppId", out var headerAppId) 
            && int.TryParse(headerAppId.ToString()["App-".Length..], out var parsedAppId))
        {
            appId = parsedAppId;
        }
    }

    if (appId is null)
    {
        context.Response.StatusCode = 404;
        return;
    }
    var app = await appRepository.FindByAppId(appId.Value);
    if (app is null)
    {
        context.Response.StatusCode = 404;
        return;
    }

    var routes = await routeRepository.List(app.Id, "", "");
    Route? route = null;
    var routeValues = new RouteValueDictionary(); 
    foreach (var r in routes)
    {
        var matcher = new TemplateMatcher(TemplateParser.Parse(r.Path), []);
        if (matcher.TryMatch(context.Request.Path, routeValues) && context.Request.Method.Equals(r.Method, StringComparison.OrdinalIgnoreCase))
        {
            route = r;
            break;
        }
    }
    
    if (route is null)
    {
        context.Response.StatusCode = 404;
        return;
    }
    
    context.Items["_App"] = app;
    context.Items["_Route"] = route;
    context.Items["_RouteValues"] = routeValues;

    await next(context);

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
    var routeValues = (RouteValueDictionary)context.Items["_RouteValues"]!;
    if (route.ResponseStatusCode > 0)
    {
        context.Response.StatusCode = route.ResponseStatusCode;
    }
    context.Response.ContentType = "application/json; charset=utf-8";
    await context.Response.WriteAsync(route.ResponseText);
});

app.Run();

