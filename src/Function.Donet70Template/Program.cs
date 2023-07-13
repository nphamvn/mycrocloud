var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<IExpectionCallback, ExpectionCallback>();
var app = builder.Build();
app.Run(async (context) =>
{
    await new RequestHandler().Hanlde(context);
});
app.Run();
