using Microsoft.EntityFrameworkCore;
using BookManagement.Domain;

namespace BookManagement.Infrastructure;

public class BookManagementDbContext : DbContext
{
    public BookManagementDbContext(DbContextOptions<BookManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>()
            .HasIndex(b => b.Title)
            .IsUnique()
            .HasFilter("IsDeleted = 0");

        modelBuilder.Entity<Book>()
            .Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        modelBuilder.Entity<Book>()
            .Property(b => b.AuthorName)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<User>()
            .Property(u => u.PasswordHash)
            .IsRequired();
    }
}
