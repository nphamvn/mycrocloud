using Microsoft.EntityFrameworkCore;
using WebApp.Migrations;

namespace WebApp.MigrationTest;

public class UnitTest
{
    [Fact]
    public void Should_Not_Has_Pending_Model_Changes()
    {
        var factory = new AppDbContextFactory();
        var dbContext = factory.CreateDbContext([]);
        var result = dbContext.Database.HasPendingModelChanges();
        Assert.False(result);
    }

    [Fact]
    public void Should_Migrate_Database()
    {
        var factory = new AppDbContextFactory();
        var dbContext = factory.CreateDbContext([]);
        dbContext.Database.Migrate();
    }
}