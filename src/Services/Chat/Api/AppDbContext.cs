using Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Api;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<Conversation> Conversations { get; set; }
    
    public DbSet<UserContact> Contacts { get; set; }
    
    public DbSet<Message> Messages { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<UserContact>()
            .HasKey(uc => new { uc.UserId, uc.ContactId });
        
        modelBuilder.Entity<ConversationUser>()
            .HasKey(cu => new { cu.UserId, cu.ConversationId });
    }
}