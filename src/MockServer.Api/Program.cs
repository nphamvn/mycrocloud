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

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions();
builder.Services.AddGlobalSettings(builder.Configuration);
builder.Services.AddOptions<VirtualHostOptions>()
    .BindConfiguration(VirtualHostOptions.Section);
builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IFactoryService, FactoryService>();
builder.Services.AddScoped<TemplateParserMatcherRouteService>();
builder.Services.AddHttpForwarder();
builder.Services.AddScoped<RoutingMiddleware>();
builder.Services.AddScoped<AuthenticationMiddleware>();
builder.Services.AddScoped<AuthorizationMiddleware>();
builder.Services.AddScoped<ConstraintValidationMiddleware>();
builder.Services.AddSingleton<ConstraintBuilder>(x => new ConstraintBuilder(ConstraintBuilder.GetDefaultConstraintMap()));  //TODO: Use ActivatorUtilities.CreateInstance
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
//TODO: Use ActivatorUtilities.CreateInstance
builder.Services.AddSingleton<DataBinderProvider>(x => new DataBinderProvider(new DataBinderProviderOptions
{
    Map = DataBinderProviderOptions.Default.Map
}));

builder.Services.AddScoped<Host>();
builder.Services.AddControllers();

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