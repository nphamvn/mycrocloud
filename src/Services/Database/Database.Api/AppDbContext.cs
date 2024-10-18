using Microsoft.EntityFrameworkCore;

namespace Database.Api;

public class AppDbContext : DbContext
{
    public DbSet<DatabaseEntity> Databases { get; set; }
}