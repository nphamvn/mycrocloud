using System.ComponentModel.DataAnnotations;
using Form.Builder.Api.Entities;
using Form.Builder.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Form.Builder.Api
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Entities.Form> Forms { get; set; } = null!;

        public DbSet<FormField> FormFields { get; set; } = null!;

        public DbSet<FormSubmission> FormSubmissions { get; set; } = null!;
        public DbSet<FormFieldValue> FormFieldValues { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FormField>()
                .OwnsOne(x => x.Details, b =>
                {
                    b.ToJson();
                    b.OwnsOne(x => x.NumberInput);
                    b.OwnsOne(x => x.TextInput);
                });
        }

        public override int SaveChanges()
        {
            Validate();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            Validate();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void Validate()
        {
            var entities = from e in ChangeTracker.Entries()
                where e.State is EntityState.Added or EntityState.Modified
                select e.Entity;
            
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity);
                Validator.ValidateObject(
                    entity,
                    validationContext,
                    validateAllProperties: true);
            }
        }
    }
}
