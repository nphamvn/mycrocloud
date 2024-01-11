using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Repositories;
using WebApp.Infrastructure.Repositories.EfCore;
using WebApp.MiniApiGateway;
using Route = WebApp.Domain.Entities.Route;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(options => { options.AddSeq(builder.Configuration["Logging:Seq:ServerUrl"]); });
builder.Services.AddHttpLogging(o => { });
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
});
builder.Services.AddScoped<IAppRepository, AppRepository>();
builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddSingleton(new ScriptCollection
{
    { "faker", File.ReadAllText("Scripts/faker.js") },
    { "handlebars", File.ReadAllText("Scripts/Handlebars.js")},
    { "lodash", File.ReadAllText("Scripts/lodash.js")}
});

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Starting up...");

app.UseHttpLogging();

app.UseLoggingMiddleware();

if (app.Environment.IsDevelopment())
{
    app.Use((context, next) =>
    {
        //Mock header. In production this header should be set from LB
        var subDomain = context.Request.Host.Host.Split(".")[0];
        context.Request.Headers.Append("X-AppId", subDomain);
        return next(context);
    });
}

app.UseAppResolverMiddleware();

app.UseRouteResolverMiddleware();

app.UseValidationMiddleware();

app.MapWhen(context => ((Route)context.Items["_Route"]!).ResponseType == "static",appBuilder => appBuilder.Run(StaticResponseHandler.Handle));

app.MapWhen(context => ((Route)context.Items["_Route"]!).ResponseType == "function", appBuilder => appBuilder.Run(FunctionHandler.Handle));

app.Run();