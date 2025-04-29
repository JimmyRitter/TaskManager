using Microsoft.EntityFrameworkCore;
using SaasTaskManager.Core.Entities;

namespace SaasTaskManager.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Email).HasMaxLength(255).IsRequired();
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.Name).HasMaxLength(100).IsRequired();
            builder.Property(u => u.HashedPassword).IsRequired();
            builder.Property(u => u.CreatedAt).IsRequired();
            builder.Property(u => u.IsEmailVerified).IsRequired();
            builder.Property(u => u.IsActive).IsRequired();
            builder.Property(u => u.EmailVerificationToken).HasMaxLength(100);
        });
    }
}