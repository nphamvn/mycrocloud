using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Filters;
using WebApp.Api.Rest;
using WebApp.Domain.Services;
using WebApp.Domain.Repositories;
using WebApp.Infrastructure.Repositories.EfCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(GlobalExceptionFilter));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(options =>
{
    options.AddSeq(builder.Configuration["Logging:Seq:ServerUrl"]);
});
builder.Services.AddHttpLogging(o => { });
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// 1. Add Authentication Services
builder.Services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    options.Authority = builder.Configuration["Authentication:Schemes:Auth0JwtBearer:Authority"];
                    options.Audience = builder.Configuration["Authentication:Schemes:Auth0JwtBearer:Audience"];
                });
builder.Services.AddAuthorization();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    //options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
});
builder.Services.AddScoped<IAppRepository, AppRepository>();
builder.Services.AddScoped<IAppService, AppService>();
builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<ILogRepository, LogRepository>();

var app = builder.Build();
app.UseHttpLogging();
app.MapHealthChecks("/healthz");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Map("api/ping", () => "pong");
app.Map("api/me", (ClaimsPrincipal user) => user.GetUserId())
    .RequireAuthorization();

app.Run();