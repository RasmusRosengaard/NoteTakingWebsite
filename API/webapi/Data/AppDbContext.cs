using Microsoft.EntityFrameworkCore;
using webapi.Models;

namespace webapi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Note> Notes => Set<Note>();
        public DbSet<Canvas> Canvases => Set<Canvas>();
        public DbSet<UserCanvas> UserCanvases => Set<UserCanvas>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Composite key for join table
            modelBuilder.Entity<UserCanvas>()
                .HasKey(uc => new { uc.UserId, uc.CanvasId });

            // ✅ User ↔ UserCanvas
            modelBuilder.Entity<UserCanvas>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCanvases)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Canvas ↔ UserCanvas
            modelBuilder.Entity<UserCanvas>()
                .HasOne(uc => uc.Canvas)
                .WithMany(c => c.UserCanvases)
                .HasForeignKey(uc => uc.CanvasId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Canvas → Notes (one-to-many)
            modelBuilder.Entity<Note>()
                .HasOne(n => n.Canvas)
                .WithMany(c => c.Notes)
                .HasForeignKey(n => n.CanvasId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Optional: Configure Role (if using enum)
            modelBuilder.Entity<UserCanvas>()
                .Property(uc => uc.Role)
                .HasConversion<string>(); // stores enum as string

            // ✅ Optional: Required fields
            modelBuilder.Entity<Canvas>()
                .Property(c => c.Title)
                .HasMaxLength(200);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasMaxLength(200);
        }
    }
}