using LostAndLocate.Chats.Models;
using LostAndLocate.Files.Models;
using LostAndLocate.LostObjects.Models;
using LostAndLocate.Reviews.Models;
using LostAndLocate.Users.Models;
using Microsoft.EntityFrameworkCore;

namespace LostAndLocate.Data;

/// <summary>
/// Entity Framework Core Context to connect to database.
/// </summary>
public sealed class LostAndLocateContext : DbContext, IDbContext
{
    public LostAndLocateContext(DbContextOptions<LostAndLocateContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; private set; } = null!;

    public DbSet<LostObject> Objects { get; private set; } = null!;

    public DbSet<Review> Reviews { get; private set; } = null!;

    public DbSet<ChatMessage> ChatMessages { get; private set; } = null!;

    public DbSet<SavedFile> Files { get; private set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Register Credentials as owned type
        modelBuilder.Entity<User>()
            .OwnsOne(u => u.Credentials);

        modelBuilder.Entity<LostObject>(b =>
        {
            // Include users when receiving objects
            b.Navigation(o => o.User).AutoInclude();
            // Register Coordinates as owned type
            b.OwnsOne(o => o.Coordinates);
        });

        modelBuilder.Entity<Review>(b =>
        {
            // Include users when receiving objects
            b.Navigation(r => r.Sender).AutoInclude();
            b.Navigation(r => r.Target).AutoInclude();
        });

        modelBuilder.Entity<ChatMessage>(b =>
        {
            // Include users when receiving objects
            b.Navigation(m => m.Sender).AutoInclude();
            b.Navigation(m => m.Target).AutoInclude();
        });
    }
}