using Lubris_API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Lubris_API.Utils
{

        public class ApplicationDbContext : IdentityDbContext
        {
            public ApplicationDbContext(DbContextOptions options) : base(options)
            {
            }


            public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            {
                var insertedEntries = this.ChangeTracker.Entries()
                                       .Where(x => x.State == EntityState.Added)
                                       .Select(x => x.Entity);
                foreach (var insertedEntry in insertedEntries)
                {
                    var auditableEntity = insertedEntry as AuditableEntity;
                    if (auditableEntity != null)
                    {
                        auditableEntity.CreatedAt = DateTime.UtcNow;
                    }
                }

                var modifiedEntries = this.ChangeTracker.Entries()
                           .Where(x => x.State == EntityState.Modified)
                           .Select(x => x.Entity);

                foreach (var modifiedEntry in modifiedEntries)
                {
                    var auditableEntity = modifiedEntry as AuditableEntity;
                    if (auditableEntity != null)
                    {
                        auditableEntity.UpdatedAt = DateTime.UtcNow;
                    }
                }

                return base.SaveChangesAsync(cancellationToken);
            }

            protected override void OnModelCreating(ModelBuilder builder)
            {
            builder.Entity<Product>()
                   .HasMany(p => p.Sales)
                   .WithOne(s => s.Product)
                   .HasForeignKey(s => s.ProductId);

            builder.Entity<Sale>()
                   .HasOne(s => s.Product)
                   .WithMany(p => p.Sales)
                   .HasForeignKey(s => s.ProductId);

            builder.Entity<Sale>()
                   .HasOne(s => s.Client)
                   .WithMany(c => c.Sales)
                   .HasForeignKey(s => s.ClientId);

            builder.Entity<Client>()
                   .HasMany(c => c.Sales)
                   .WithOne(s => s.Client)
                   .HasForeignKey(s => s.ClientId);

            base.OnModelCreating(builder);
            }

            public DbSet<ApplicationUser> ApplicationUsers { get; set; }
            public DbSet<Product> Products { get; set; }
            public DbSet<Sale> Sales { get; set; }
            public DbSet<Client> Clients { get; set; }
           



    }
}
