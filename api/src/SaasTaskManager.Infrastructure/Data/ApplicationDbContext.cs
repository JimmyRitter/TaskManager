using Microsoft.EntityFrameworkCore;
using SaasTaskManager.Core.Entities;
using Task = SaasTaskManager.Core.Entities.Task;

namespace SaasTaskManager.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<List> Lists { get; set; }
    public DbSet<ListShare> ListShares { get; set; }
    public DbSet<Task> Tasks { get; set; }


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

        modelBuilder.Entity<List>(builder =>
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Name).HasMaxLength(100).IsRequired();
            builder.Property(u => u.Description).HasMaxLength(255);
            builder.Property(u => u.Category);
            builder.Property(u => u.OwnerId);
            builder.Property(u => u.CreatedAt).IsRequired();
            builder.Property(u => u.UpdatedAt);
            
            // Configure relationship with Tasks
            builder.HasMany(l => l.Tasks)
                .WithOne()
                .HasForeignKey(t => t.ListId)
                .OnDelete(DeleteBehavior.Cascade);
        });


        modelBuilder.Entity<ListShare>(builder =>
        {
            builder.HasKey(ls => ls.Id);
            builder.Property(ls => ls.ListId).IsRequired();
            builder.Property(ls => ls.SharedWithUserId).IsRequired();
            builder.Property(ls => ls.Permission).IsRequired();
            builder.Property(ls => ls.CreatedAt).IsRequired();
            builder.Property(ls => ls.UpdatedAt).IsRequired();
        });

        modelBuilder.Entity<Task>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Description).IsRequired();
            builder.Property(t => t.Priority).IsRequired();
            builder.Property(t => t.IsCompleted);
            builder.Property(t => t.ListId).IsRequired();
            builder.Property(t => t.Order).IsRequired();
            builder.Property(t => t.DueDate);
            builder.Property(t => t.CreatedAt).IsRequired();
            builder.Property(t => t.UpdatedAt);
            builder.Property(t => t.DeletedAt);
        });
    }
}