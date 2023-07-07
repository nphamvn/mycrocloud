using MockServer.Domain.Repositories;
using MockServer.Domain.Settings;
using MockServer.Infrastructure.Repositories.PostgreSql;
using MockServer.MockResponder.Extensions;
using MockServer.MockResponder.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<PostgresSettings>(builder.Configuration.GetSection("Database:Application"));
builder.Services.AddScoped<IWebApplicationRouteRepository, WebApplicationRouteRepository>();
builder.Services.AddScoped<IHttpResponseRetriever, HttpResponseRetriever>();
var routeIdKey = "__RouteId";
var app = builder.Build();
app.Use( async (context, next) =>
{
    if (context.Request.Headers.TryGetValue(routeIdKey, out var routeIdHeaderValue))
    {
        if (int.TryParse(routeIdHeaderValue, out var routeId))
        {
            context.Items[routeIdKey] = routeId;
            context.Request.Headers.Remove(routeIdKey);
            await next.Invoke(context);
        }
    }
});
app.Run(async context => {
    int routeId = (int)context.Items[routeIdKey];
    var retriever = context.RequestServices.GetService<IHttpResponseRetriever>(); 
    var responseMessage = await retriever.GetResponseMessage(routeId, context);
    await context.WriteResponseMessage(responseMessage);
});
app.Run();