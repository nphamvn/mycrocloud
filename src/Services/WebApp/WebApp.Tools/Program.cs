using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApp.Infrastructure.Repositories.EfCore;

Console.WriteLine("Type 'yes' to confirm the operation.");
var input = Console.ReadLine();
if (input != "yes")
{
    Console.WriteLine("Operation cancelled.");
    return;
}

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var options = new DbContextOptionsBuilder<AppDbContext>();
options.UseNpgsql(configuration.GetConnectionString("PostgreSQL"));
using var context = new AppDbContext(options.Options);
var apps = context.Apps.ToList();
foreach (var app in apps)
{
    app.CorsSettings ??= new();
}
context.Apps.UpdateRange(apps);
context.SaveChanges();