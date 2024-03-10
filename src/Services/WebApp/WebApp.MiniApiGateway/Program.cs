using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Repositories;
using WebApp.Infrastructure.Repositories.EfCore;
using WebApp.MiniApiGateway;
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
app.UseWhen(context => context.Request.Host.Host == builder.Configuration["Host"], (config) =>
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
    app.Use((context, next) =>
    {
        //Mock header. In production this header should be set from LB
        var host = context.Request.Host.Host;
        var pattern = builder.Configuration["HostRegex"]!;
        var match = System.Text.RegularExpressions.Regex.Match(host, pattern);
        if (match.Success)
        {
            var appId = int.Parse(match.Groups[1].Value);
            var source = builder.Configuration["AppIdSource"]!.Split(":")[0];
            var name = builder.Configuration["AppIdSource"]!.Split(":")[1];
            if (source == "Header")
            {
                context.Request.Headers.Append(name, appId.ToString());
            }
        }
        return next(context);
    });
}

app.UseAppResolverMiddleware();

app.UseCorsMiddleware();

app.UseRouteResolverMiddleware();

app.UseAuthenticationMiddleware();

app.UseAuthorizationMiddleware();

app.UseValidationMiddleware();

app.MapWhen(context => ((Route)context.Items["_Route"]!).ResponseType == "static",
    appBuilder => appBuilder.Run(StaticResponseHandler.Handle));

app.MapWhen(context => ((Route)context.Items["_Route"]!).ResponseType == "function",
    appBuilder => appBuilder.Run(FunctionHandler.Handle));

app.Run();