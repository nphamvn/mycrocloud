using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MockServer.Core.Repositories;
using MockServer.Infrastructure.Repositories.PostgreSql;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json");
builder.Services.AddOcelot();
// builder.Services.AddScoped<IWebApplicationRepository, WebApplicationRepository>();
// builder.Services.AddScoped<IWebApplicationRouteRepository, WebApplicationRouteRepository>();
var app = builder.Build();
app.UseOcelot().Wait();
app.Run();