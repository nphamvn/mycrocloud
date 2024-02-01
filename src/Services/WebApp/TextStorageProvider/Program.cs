using System.Security.Claims;
using TextStorageProvider;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication()
                .AddScheme<ConnectionStringAuthenticationSchemOptions, ConnectionStringAuthenticationHandler>("ConnectionStringAuthentication", null);

builder.Services.AddAuthorization();
builder.Services.AddHttpClient("TextStorageProvider", c =>
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

app.MapGet("/", async (ClaimsPrincipal user) =>
{
    var name = user.Claims.First(c => c.Type == "Name").Value;
    var text = await File.ReadAllTextAsync(name);
    return text;
}).RequireAuthorization();

app.MapPost("/", async (ClaimsPrincipal user, HttpContext context) =>
{
    var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
    var name = user.Claims.First(c => c.Type == "Name").Value;
    File.WriteAllText(name, body);
    return Results.Ok();
}).RequireAuthorization();

app.Run();