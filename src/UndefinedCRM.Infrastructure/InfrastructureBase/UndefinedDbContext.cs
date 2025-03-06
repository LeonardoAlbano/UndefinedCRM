using Microsoft.EntityFrameworkCore;
using UndefinedCRM.Domain;
using UndefinedCRM.Domain.Entities;

namespace UndefinedCRM.Infrastructure.InfrastructureBase
{
    public class UndefinedDbContext(DbContextOptions<UndefinedDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Email).IsRequired();
                entity.HasIndex(u => u.Email).IsUnique();
            });
        }
    }
}