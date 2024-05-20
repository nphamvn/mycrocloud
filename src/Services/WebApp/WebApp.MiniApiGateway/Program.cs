using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;
using WebApp.Infrastructure;
using WebApp.Infrastructure.Repositories;
using WebApp.MiniApiGateway;
using WebApp.MiniApiGateway.Middlewares;
using File = System.IO.File;
using Route = WebApp.Domain.Entities.Route;

var builder = WebApplication.CreateBuilder(args);
ConfigurationHelper.Initialize(builder.Configuration);
builder.Services.AddLogging(options =>
{
    options.AddSeq(builder.Configuration["Logging:Seq:ServerUrl"], builder.Configuration["Logging:Seq:ApiKey"]);
});
builder.Services.AddHttpLogging(o => { });

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
        //.LogTo(Console.WriteLine, LogLevel.Information)
        //.EnableSensitiveDataLogging()
        ;
});
builder.Services.AddScoped<IAppRepository, AppRepository>();
builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<ILogRepository, LogRepository>();
builder.Services.AddSingleton(new ScriptCollection
{
    { "faker", File.ReadAllText("Scripts/faker.js") },
    { "handlebars", File.ReadAllText("Scripts/handlebars.min-v4.7.8.js")},
    { "lodash", File.ReadAllText("Scripts/lodash.min.js")}
});
builder.Services.AddSingleton<ICachedOpenIdConnectionSigningKeys, MemoryCachedOpenIdConnectionSigningKeys>();
builder.Services.AddHealthChecks();
var app = builder.Build();

app.UseHttpLogging();
app.UseWhen(context => context.Request.Host.Host == builder.Configuration["Host"], config =>
{
    config.UseHealthChecks("/healthz");
    config.Run(async context =>
    {
        await context.Response.CompleteAsync();
    });
});

app.UseLoggingMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseMiddleware<DevAppIdResolverMiddleware>();
}

app.UseAppResolverMiddleware();

app.UseCorsMiddleware();

app.UseRouteResolverMiddleware();

app.UseAuthenticationMiddleware();

app.UseAuthorizationMiddleware();

app.UseValidationMiddleware();

app.MapWhen(context => ((Route)context.Items["_Route"]!).ResponseType == ResponseType.Static,
    appBuilder => appBuilder.Run(StaticResponseHandler.Handle));

app.MapWhen(context => ((Route)context.Items["_Route"]!).ResponseType == ResponseType.StaticFile,
    appBuilder => appBuilder.Run(FileResponseHandler.Handle));

app.MapWhen(context => ((Route)context.Items["_Route"]!).ResponseType == ResponseType.Function,
    appBuilder => appBuilder.Run(FunctionHandler.Handle));

app.Run();