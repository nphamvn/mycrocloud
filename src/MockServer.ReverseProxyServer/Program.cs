using MockServer.Core.Repositories;
using MockServer.Core.Services;
using MockServer.Infrastructure.Repositories;
using MockServer.ReverseProxyServer.Extentions;
using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Middlewares;
using MockServer.ReverseProxyServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions();
builder.Services.AddGlobalSettings(builder.Configuration);
builder.Services.AddScoped<RouteValidation>();
builder.Services.AddScoped<Authentication>();
builder.Services.AddScoped<ConstraintsValidation>();
builder.Services.AddMemoryCache();
builder.Services.AddTransient<IRequestRepository, RequestRepository>();
builder.Services.AddTransient<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IRequestHandler, FixedRequestHandler>();
builder.Services.AddScoped<IRequestHandler, ForwardingRequestHandler>();
builder.Services.AddScoped<IRequestHandlerFactory, RequestHandlerFactory>();
builder.Services.AddScoped<ICacheService, MemoryCacheService>();
builder.Services.AddScoped<IRouteResolver, TemplateParserMatcherRouteService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<RequestHandler>();
builder.Services.AddModelBinderProvider(options =>
{
    options.Map = DataBinderProviderOptions.Default.Map;
});

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
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapTestPaths();
}
//Enable CORSE
app.UseCors(MyAllowSpecificOrigins);
//Validate route
app.UseRouteValidation();
//Authenticate
app.UseRequestAuthentication();
//Validate
app.UseConstraintsValidation();
app.Run(async context =>
{
    var handler = context.RequestServices.GetRequiredService<RequestHandler>();
    await handler.Handle(context);
});
app.Run();