using Serilog;
using WebApp.Api.Grpc.Services;
using WebApp.Domain.Repositories;
using WebApp.Infrastructure.Repositories.PostgreSql;

var builder = WebApplication.CreateBuilder(args);
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.Configure<PostgresDatabaseOptions>(builder.Configuration.GetSection("PostgresDatabaseOptions"));

builder.Services.AddScoped<IAppRepository, AppRepository>();
builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<IAuthenticationSchemeRepository, AuthenticationSchemeRepository>();
builder.Services.AddScoped<IAuthorizationPolicyRepository, AuthorizationPolicyRepository>();

builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<AppService>();
app.MapGrpcService<RouteService>();
app.MapGrpcService<AuthenticationService>();

app.Run();