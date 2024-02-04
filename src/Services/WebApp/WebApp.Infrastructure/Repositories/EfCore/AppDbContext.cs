using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Repositories.EfCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<App> Apps { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<RouteValidation> RouteValidations { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<AuthenticationScheme> AuthenticationSchemes { get; set; }
    public DbSet<Variable> Variables { get; set; }
    public DbSet<TextStorage> TextStorages { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<App>()
            .OwnsOne(app => app.Settings, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ToJson();
            });
        modelBuilder.Entity<App>()
            .OwnsOne(app => app.CorsSettings, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.ToJson();
            });

        modelBuilder.Entity<Route>().OwnsMany(route => route.ResponseHeaders,
            ownedNavigationBuilder => { ownedNavigationBuilder.ToJson(); });
        _ = modelBuilder.Entity<RouteValidation>()
            .Property(p => p.Rules)
            .HasConversion(v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, new JsonSerializerOptions()));
    }
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        AddTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void AddTimestamps()
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (
                e.State == EntityState.Added
                || e.State == EntityState.Modified));
        foreach (var entityEntry in entries)
        {
            if (entityEntry.State == EntityState.Added)
            {
                ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
            }
            else
            {
                ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}