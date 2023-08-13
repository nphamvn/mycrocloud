using WebApp.Api.Grpc.Services;
using WebApp.Domain.Repositories;
using WebApp.Infrastructure.Repositories.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<PostgresDatabaseOptions>(builder.Configuration.GetSection("PostgresDatabaseOptions"));

builder.Services.AddScoped<IWebAppRepository, WebAppRepository>();
builder.Services.AddScoped<IWebAppRouteRepository, WebAppRouteRepository>();
builder.Services.AddScoped<IWebAppAuthenticationSchemeRepository, WebAppAuthenticationSchemeRepository>();
builder.Services.AddScoped<IWebAppAuthorizationPolicyRepository, WebAppAuthorizationPolicyRepository>();

builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<WebAppService>();
app.MapGrpcService<WebAppRouteService>();
app.MapGrpcService<WebAppAuthenticationService>();

app.Run();