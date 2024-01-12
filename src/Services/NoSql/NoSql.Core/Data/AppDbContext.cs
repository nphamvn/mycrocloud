using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NoSql.Core.Entities;

namespace NoSql.Core.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Server> Servers { get; set; } = null!;
    public DbSet<Database> Databases { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Database>()
            .Property(p => p.Data)
            .HasConversion(v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                v => JsonSerializer.Deserialize<object>(v, new JsonSerializerOptions()));
        modelBuilder.Entity<Database>()
            .Property(p => p.Schema)
            .HasConversion(v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                v => JsonSerializer.Deserialize<object>(v, new JsonSerializerOptions()));
    }
}