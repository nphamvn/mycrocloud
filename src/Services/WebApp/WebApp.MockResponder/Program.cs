using WebApp.Domain.Repositories;
using WebApp.Infrastructure.Repositories.PostgreSql;
using MockServer.MockResponder.Extensions;
using MockServer.MockResponder.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<PostgresDatabaseOptions>(builder.Configuration.GetSection("Database:Application"));
builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<IHttpResponseRetriever, HttpResponseRetriever>();
const string RouteIdKey = "__RouteId";
var app = builder.Build();
app.Use( async (context, next) =>
{
    if (context.Request.Headers.TryGetValue(RouteIdKey, out var routeIdHeaderValue))
    {
        if (int.TryParse(routeIdHeaderValue, out var routeId))
        {
            context.Items[RouteIdKey] = routeId;
            context.Request.Headers.Remove(RouteIdKey);
            await next.Invoke(context);
        }
    }
});
app.Run(async context => {
    var routeId = (int)context.Items[RouteIdKey]!;
    var retriever = context.RequestServices.GetService<IHttpResponseRetriever>(); 
    var responseMessage = await retriever.GetResponseMessage(routeId, context);
    await context.WriteResponseMessage(responseMessage);
});
app.Run();