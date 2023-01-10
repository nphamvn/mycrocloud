using MockServer.Core.Repositories;
using MockServer.Infrastructure.Repositories;
using MockServer.ReverseProxyServer.Extentions;
using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Middlewares;
using MockServer.ReverseProxyServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGlobalSettings(builder.Configuration);
builder.Services.AddScoped<RequestValidation>();
builder.Services.AddMemoryCache();
builder.Services.AddTransient<IRequestRepository, RequestRepository>();
builder.Services.AddTransient<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IRequestHandler, FixedRequestHandler>();
builder.Services.AddScoped<IRequestHandler, ForwardingRequestHandler>();
builder.Services.AddScoped<IRequestHandlerFactory, RequestHandlerFactory>();
builder.Services.AddScoped<ICacheService, MemoryCacheService>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<RequestHandler>();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins, policy =>
    {
        policy.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});
var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapTestPaths();
}
//Use RequestValidator to check if request is existing and check if request is allowed (private or public)
app.UseRequestValidator();
app.Run(async context =>
{
    var handler = context.RequestServices.GetRequiredService<RequestHandler>();
    await handler.Handle(context);
});
app.Run();