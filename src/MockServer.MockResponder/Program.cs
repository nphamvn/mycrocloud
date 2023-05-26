using MockServer.Core.Repositories;
using MockServer.Core.Settings;
using MockServer.Infrastructure.Repositories.PostgreSql;
using MockServer.MockResponder.Extensions;
using MockServer.MockResponder.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<PostgresSettings>(builder.Configuration.GetSection("Database:Application"));
builder.Services.AddScoped<IWebApplicationRouteRepository, WebApplicationRouteRepository>();
builder.Services.AddScoped<IHttpResponseRetriever, HttpResponseRetriever>();
var app = builder.Build();
app.MapGet("/{routeId:int}", async (int routeId, HttpContext context, IHttpResponseRetriever service) =>
{
    var responseMessage = await service.GetResponseMessage(routeId);
    await context.WriteResponseMessage(responseMessage);
});

app.Run();