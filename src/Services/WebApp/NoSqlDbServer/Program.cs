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
builder.Services.AddHttpClient("NoSqlDbServer", c =>
{
    c.BaseAddress = new Uri("http://localhost:5148");
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("_docs", async (string name, ClaimsPrincipal user) =>
{
    var text = await File.ReadAllTextAsync(name + ".json");
    return text;
}).RequireAuthorization();

app.MapPost("_docs", async (string name, ClaimsPrincipal user, HttpContext context) =>
{
    var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
    File.WriteAllText(name + ".json", body);
    return Results.Ok();
}).RequireAuthorization();

app.Run();