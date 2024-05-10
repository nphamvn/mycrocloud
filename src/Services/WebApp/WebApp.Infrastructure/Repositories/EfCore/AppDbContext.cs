using Microsoft.EntityFrameworkCore;
using WebApp.Domain.Entities;
using File = WebApp.Domain.Entities.File;

namespace WebApp.Infrastructure.Repositories.EfCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<App> Apps { get; set; }
    
    public DbSet<RouteFolder> RouteFolders { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<Folder> Folders { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<AuthenticationScheme> AuthenticationSchemes { get; set; }

    public DbSet<ApiKey> ApiKeys { get; set; }
    
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

        modelBuilder.Entity<Route>()
            .HasOne(route => route.File)
            .WithMany()
            .OnDelete(DeleteBehavior.SetNull);
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