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
        public DbSet<UserNote> UserNotes => Set<UserNote>(); // join table

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship
            modelBuilder.Entity<UserNote>()
                .HasKey(un => new { un.UserId, un.NoteId }); // composite primary key

            modelBuilder.Entity<UserNote>()
                .HasOne(un => un.User)
                .WithMany(u => u.UserNotes)
                .HasForeignKey(un => un.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserNote>()
                .HasOne(un => un.Note)
                .WithMany(n => n.UserNotes)
                .HasForeignKey(un => un.NoteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}