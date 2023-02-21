using MockServer.Core.Repositories;
using MockServer.Core.Services;
using MockServer.Infrastructure.Repositories;
using MockServer.Api.Extentions;
using MockServer.Api.Interfaces;
using MockServer.Api.Middlewares;
using MockServer.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOptions();
builder.Services.AddGlobalSettings(builder.Configuration);
builder.Services.AddScoped<IFactoryService, FactoryService>();
builder.Services.AddScoped<RouteService>();
builder.Services.AddScoped<Authentication>();
builder.Services.AddScoped<Authorization>();
builder.Services.AddScoped<ConstraintsValidation>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IDatabaseRepository, DatabaseRespository>();
builder.Services.AddScoped<IRequestHandler, FixedRequestHandler>();
builder.Services.AddScoped<IRequestHandler, ForwardingRequestHandler>();
builder.Services.AddScoped<IRequestHandlerFactory, RequestHandlerFactory>();
builder.Services.AddScoped<ICacheService, MemoryCacheService>();
builder.Services.AddScoped<IRouteResolver, TemplateParserMatcherRouteService>();
builder.Services.AddMemoryCache();
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
app.UseRouteResolver();
//Authenticate
app.UseRequestAuthentication();
//Authorize
app.UseRequestAuthorization();
//Validate
app.UseConstraintsValidation();
app.Run(async context =>
{
    var handler = context.RequestServices.GetRequiredService<RequestHandler>();
    await handler.Handle(context);
});
app.Run();