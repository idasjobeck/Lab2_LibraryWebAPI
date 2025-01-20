using Lab2_LibraryWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Lab2_LibraryWebAPI.Data
{
    public class BooksDbContext : DbContext
    {
        public BooksDbContext(DbContextOptions<BooksDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Edition> Editions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<AuthorBook> AuthorBook { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.ISBN)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.CardNumber, "IX_Users_CardNumber")
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email, "IX_Users_Email")
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasMany(e => e.Loans)
                .WithOne(e => e.User)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Book>()
                .HasMany(e => e.Loans)
                .WithOne(e => e.Book)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Author>()
                .HasMany(e => e.Books)
                .WithMany(e => e.Authors)
                .UsingEntity<AuthorBook>();
            modelBuilder.Entity<Book>()
                .ToTable(b => b.HasCheckConstraint("CK_Book_AvailableQty", "AvailableQty <= TotalQty"));
        }
    }
}
