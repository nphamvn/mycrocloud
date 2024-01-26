var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/nosqldb/connect", (int appId, string connectionString) => new { Token = "token" })
    .WithName("ConnectNoSqlDb")
    .WithOpenApi();

app.MapGet("/nosqldb/collection", (string token, string name) =>
    {
        return new object[]
        {
            new
            {
                Id = 1,
                Name = "Item 1"
            },
            new
            {
                Id = 2,
                Name = "Item 2"
            }
        };
    })
    .WithName("GetCollection")
    .WithOpenApi();

app.Run();