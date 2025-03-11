using Microsoft.EntityFrameworkCore;
using UndefinedCRM.Domain;
using UndefinedCRM.Domain.Entities;

namespace UndefinedCRM.Infrastructure.InfrastructureBase;

public class UndefinedDbContext(DbContextOptions<UndefinedDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Client> Clients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired();
            entity.HasIndex(u => u.Email).IsUnique();
        });
        
        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired();
            entity.Property(c => c.Surname).IsRequired();
            
            // Define relationship with User
            entity.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}