using Microsoft.AspNetCore.Mvc;
using MockServer.Core.Repositories;
using MockServer.Core.Settings;
using MockServer.Infrastructure.Repositories;
using MockServer.ReverseProxyServer.Extentions;
using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Middlewares;
using MockServer.ReverseProxyServer.Models;
using MockServer.ReverseProxyServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGlobalSettings(builder.Configuration);
builder.Services.AddTransient<IRequestRepository, RequestRepository>();
builder.Services.AddTransient<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IRequestHandler, FixedRequestHandler>();
builder.Services.AddScoped<IRequestHandler, ForwardingRequestHandler>();
builder.Services.AddScoped<IRequestHandlerFactory, RequestHandlerFactory>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<RequestHandler>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}
//Use RequestValidator to check if request is existing and check if request is allowed (private or public)
app.UseRequestValidator();
app.Run(async context =>
{
    var handler = context.RequestServices.GetRequiredService<RequestHandler>();
    await handler.Handle(context);
});
app.Run();