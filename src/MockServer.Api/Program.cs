using MockServer.Core.Repositories;
using MockServer.Core.Services;
using MockServer.Infrastructure.Repositories;
using MockServer.Api.Extentions;
using MockServer.Api.Interfaces;
using MockServer.Api.Middlewares;
using MockServer.Api.Services;
using MockServer.Api.TinyFramework;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions();
builder.Services.AddGlobalSettings(builder.Configuration);
builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IFactoryService, FactoryService>();
builder.Services.AddScoped<TemplateParserMatcherRouteService>();
builder.Services.AddScoped<RoutingMiddleware>();
builder.Services.AddScoped<AuthenticationMiddleware>();
builder.Services.AddScoped<AuthorizationMiddleware>();
builder.Services.AddScoped<ConstraintValidationMiddleware>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IDatabaseRepository, DatabaseRespository>();
builder.Services.AddScoped<IRequestHandler, FixedRequestHandler>();
builder.Services.AddScoped<IRequestHandler, ForwardingRequestHandler>();
builder.Services.AddScoped<IRequestHandlerFactory, RequestHandlerFactory>();
builder.Services.AddScoped<ICacheService, MemoryCacheService>();
builder.Services.AddScoped<IRouteResolver, TemplateParserMatcherRouteService>();
builder.Services.AddScoped<RequestHandler>();
builder.Services.AddScoped<WebApplicationResolver>();
builder.Services.AddModelBinderProvider(options =>
{
    options.Map = DataBinderProviderOptions.Default.Map;
});

builder.Services.AddScoped<Container>();

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
    //app.MapTestPaths();
    app.Use(async (context, next) =>
    {
        var host = context.Request.Host.Host;
        context.Request.Host = new HostString("nampham.todos.mockserver.com", context.Request.Host.Port ?? 5000);
        await next.Invoke(context);
    });
}
//Enable CORS
app.UseCors(MyAllowSpecificOrigins);
//Validate route
app.UseWebApplicationResolver();

app.Run(async (context) =>
{
    var container = context.RequestServices.GetService<Container>();
    await container.Run();
});
app.Run();