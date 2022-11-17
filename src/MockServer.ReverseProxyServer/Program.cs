using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Middlewares;
using MockServer.ReverseProxyServer.Models;
using MockServer.ReverseProxyServer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<IRequestServices, RequestServices>();
builder.Services.AddScoped<IRequestHandler, FixedRequestHandler>();
builder.Services.AddScoped<IRequestHandler, ForwardingRequestHandler>();
builder.Services.AddTransient<IRequestHandlerFactory, RequestHandlerFactory>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}
//Use RequestValidator to check if request is existing and check if request is allowed (private or public)
app.UseRequestValidator();

app.Use(async (context, next) =>
{
    var request = MapRequest(context.Request);
    Console.WriteLine(request.ToString());
    context.Items[nameof(RequestModel)] = request;
    await next.Invoke();
});
var handler = new RequestHandler();
app.Run(handler.Handle);
app.Run();

RequestModel MapRequest(HttpRequest request)
{
    return new RequestModel
    {
        Username = "nampham",
        Method = request.Method,
        Path = request.Path.Value
    };
}
