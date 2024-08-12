using bookstoreapi.Models;
using Microsoft.EntityFrameworkCore;

public class BookstoreContext : DbContext
{
    public BookstoreContext(DbContextOptions<BookstoreContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Genre> Genres { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Configure Authors
    modelBuilder.Entity<Author>()
        .ToTable("Authors")
        .HasKey(a => a.AuthorId);

    modelBuilder.Entity<Author>()
        .Property(a => a.AuthorId)
        .HasColumnName("author_id");

    modelBuilder.Entity<Author>()
        .Property(a => a.AuthorName)
        .HasColumnName("authorname")
        .HasMaxLength(255)
        .IsRequired();

    modelBuilder.Entity<Author>()
        .Property(a => a.Biography)
        .HasColumnName("biography");

    modelBuilder.Entity<Author>()
        .Property(a => a.CreatedAt)
        .HasColumnName("createdAt")
        .IsRequired();

    modelBuilder.Entity<Author>()
        .Property(a => a.UpdatedAt)
        .HasColumnName("updatedAt")
        .IsRequired();

    // Configure Genres
    modelBuilder.Entity<Genre>()
        .ToTable("Genres")
        .HasKey(g => g.GenreId);

    modelBuilder.Entity<Genre>()
        .Property(g => g.GenreId)
        .HasColumnName("genre_id");

    modelBuilder.Entity<Genre>()
        .Property(g => g.GenreName)
        .HasColumnName("genre_name")
        .HasMaxLength(255)
        .IsRequired();

    modelBuilder.Entity<Genre>()
        .Property(g => g.CreatedAt)
        .HasColumnName("createdAt")
        .IsRequired();

    modelBuilder.Entity<Genre>()
        .Property(g => g.UpdatedAt)
        .HasColumnName("updatedAt")
        .IsRequired();

    // Configure Books
    modelBuilder.Entity<Book>()
        .ToTable("Books")
        .HasKey(b => b.BookId);

    modelBuilder.Entity<Book>()
        .Property(b => b.BookId)
        .HasColumnName("book_id");

    modelBuilder.Entity<Book>()
        .Property(b => b.Title)
        .HasColumnName("title")
        .HasMaxLength(255)
        .IsRequired();

    modelBuilder.Entity<Book>()
        .Property(b => b.AuthorId)
        .HasColumnName("author_id");

    modelBuilder.Entity<Book>()
        .Property(b => b.GenreId)
        .HasColumnName("genre_id");

    modelBuilder.Entity<Book>()
        .Property(b => b.Price)
        .HasColumnName("price")
        .HasColumnType("decimal(10,0)")
        .IsRequired();

    modelBuilder.Entity<Book>()
        .Property(b => b.PublicationDate)
        .HasColumnName("publication_date")
        .HasDefaultValue(null);

    modelBuilder.Entity<Book>()
        .Property(b => b.Image)
        .HasColumnName("image")
        .HasMaxLength(255)
        .HasDefaultValue(null);

    modelBuilder.Entity<Book>()
        .Property(b => b.CreatedAt)
        .HasColumnName("createdAt")
        .IsRequired();

    modelBuilder.Entity<Book>()
        .Property(b => b.UpdatedAt)
        .HasColumnName("updatedAt")
        .IsRequired();

    // Configure relationships
    modelBuilder.Entity<Book>()
        .HasOne(b => b.Author)
        .WithMany(a => a.Books)
        .HasForeignKey(b => b.AuthorId)
        .OnDelete(DeleteBehavior.Restrict);

    modelBuilder.Entity<Book>()
        .HasOne(b => b.Genre)
        .WithMany(g => g.Books)
        .HasForeignKey(b => b.GenreId)
        .OnDelete(DeleteBehavior.Restrict);
}

}
