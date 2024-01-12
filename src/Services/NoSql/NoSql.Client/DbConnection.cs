using Microsoft.EntityFrameworkCore;
using NoSql.Core.Data;
using NoSql.Core.Entities;

namespace NoSql.Client;
public class DbConnection(string connectionString)
{
    public Guid ConnectionId { get; set; }
    public void Connect()
    {
        Console.WriteLine(connectionString);
        var array = connectionString.Split(":");
        var serverId = array[0];
        var loginId = array[1];
        var password = array[2];
        var database = array[3];
        Console.WriteLine($"{serverId}:{loginId}:{password}:{database}");
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("Host=pgm-0iwyk0293g13e531io.pgsql.japan.rds.aliyuncs.com;Username=nampham;Password=6PUrTq9mebhLep;Database=dev-mycrocloud");
        using var dbContext = new AppDbContext(optionsBuilder.Options);
        // var connection = dbContext
        //     .Servers
        //     .Include(s => s.Databases)
        //     .FirstOrDefault(server => server.Id == serverId && server.LoginId == loginId && server.Password == password
        //                               && server.Databases.Any(db => db.Name == database));

        var servers = dbContext.Servers.Where(s => s.Name == serverId);
        var db = dbContext.Databases.FirstOrDefault(db => db.ServerId == serverId && db.Name == database);
        if (db is not null)
        {
            ConnectionId = Guid.NewGuid();
        }
        else
        {
            throw new NotSupportedException();
        }
    }
    public Database Read()
    {
        ArgumentNullException.ThrowIfNull(_database);
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("Host=pgm-0iwyk0293g13e531io.pgsql.japan.rds.aliyuncs.com;Username=nampham;Password=6PUrTq9mebhLep;Database=dev-mycrocloud");
        using var dbContext = new AppDbContext(optionsBuilder.Options);
         _database = dbContext.Databases.First(db => db == _database);
        return _database;
    }
    public void Write(object data)
    {
        ArgumentNullException.ThrowIfNull(_database);
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql("Host=pgm-0iwyk0293g13e531io.pgsql.japan.rds.aliyuncs.com;Username=nampham;Password=6PUrTq9mebhLep;Database=dev-mycrocloud");
        using var dbContext = new AppDbContext(optionsBuilder.Options);
        _database = dbContext.Databases.First(db => db == _database);
        _database.Data = data;
        dbContext.SaveChanges();
    }
}