using System.Security.Claims;
using NoSqlDbServer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication()
                .AddScheme<ConnectionStringAuthenticationSchemOptions, ConnectionStringAuthenticationHandler>("ConnectionStringAuthentication", null);

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("_docs", (string name, ClaimsPrincipal user) =>
{
    return "Hello World!" + user.Claims.FirstOrDefault(c => c.Type == "DatabaseId")?.Value;
}).RequireAuthorization();

app.Run();