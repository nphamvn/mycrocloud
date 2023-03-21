using MockServer.Core.Repositories;
using MockServer.Core.Services;
using MockServer.Infrastructure.Repositories;
using MockServer.Api.Extentions;
using MockServer.Api.Interfaces;
using MockServer.Api.Middlewares;
using MockServer.Api.Services;
using MockServer.Api.TinyFramework;
using MockServer.Api.TinyFramework.DataBinding;
using MockServer.Api.Options;
using Host = MockServer.Api.TinyFramework.Host;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Text.Json;
using MockServer.Core.Databases;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IFactoryService, FactoryService>();
builder.Services.AddOptions();
builder.Services.AddGlobalSettings(builder.Configuration);
builder.Services.AddOptions<VirtualHostOptions>()
                .BindConfiguration(VirtualHostOptions.Section);
builder.Services.Configure<DatabaseAdapterOptions>(options =>
{
    options.JsonSerializerOptions = new JsonSerializerOptions
    {
        WriteIndented = true
    };
});

builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpForwarder();
builder.Services.AddScoped<RoutingMiddleware>();
builder.Services.AddScoped<AuthorizationMiddleware>();
builder.Services.AddSingleton<FromQueryDataBinder>();
builder.Services.AddSingleton<FromHeaderDataBinder>();
builder.Services.AddSingleton<FromBodyDataBinder>();
builder.Services.AddScoped<RequestValidationMiddleware>();
builder.Services.AddSingleton<ConstraintBuilder>(x =>
{
    return ActivatorUtilities.CreateInstance<ConstraintBuilder>(x, ConstraintBuilder.GetDefaultConstraintMap());
});
builder.Services.AddScoped<IWebApplicationAuthenticationSchemeRepository, WebApplicationAuthenticationSchemeRepository>();
builder.Services.AddScoped<IWebApplicationRouteRepository, WebApplicationRouteRepository>();
builder.Services.AddScoped<IWebApplicationRepository, WebApplicationRepository>();
builder.Services.AddScoped<IDatabaseRepository, DatabaseRespository>();
builder.Services.AddScoped<ICacheService, MemoryCacheService>();
builder.Services.AddScoped<IRouteResolver, TemplateParserMatcherRouteService>();
builder.Services.AddScoped<WebApplicationResolver>();
builder.Services.AddSingleton<DataBinderProvider>(x =>
{
    return ActivatorUtilities.CreateInstance<DataBinderProvider>(x, new DataBinderProviderOptions
    {
        Map = DataBinderProviderOptions.Default.Map
    });
});

builder.Services.AddScoped<Host>();
builder.Services.AddControllers();
// If using Kestrel:
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
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
    var virtualHostOptions = builder.Configuration.GetSection(VirtualHostOptions.Section).Get<VirtualHostOptions>();
    if (virtualHostOptions.Enabled)
    {
        app.Use(async (context, next) =>
        {
            context.Request.Host = new HostString(virtualHostOptions.Host, virtualHostOptions.Port);
            await next.Invoke(context);
        });
    }
}
//Enable CORS
app.UseCors(MyAllowSpecificOrigins);
//Validate route
app.UseWebApplicationResolver();
app.Run(async (context) =>
{
    var host = context.RequestServices.GetService<Host>();
    await host.Run();
});
app.Run();