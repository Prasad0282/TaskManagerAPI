using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure relationships
        modelBuilder.Entity<Project>()
            .HasOne(p => p.User)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
            
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.NoAction);
            
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Seed demo user (Password: Demo@123)
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "demo",
                Email = "demo@example.com",
                PasswordHash = "$2a$11$K7Q5pY8z8z8z8z8z8z8z8uOZ5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5Z5", // Placeholder - will be overwritten
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}