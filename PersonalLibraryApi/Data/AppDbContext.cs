using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PersonalLibraryApi.Data.Models;

namespace PersonalLibraryApi.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book_Author>()
             .HasOne(b => b.Book)
             .WithMany(ba => ba.Book_Authors)
             .HasForeignKey(bi => bi.BookId);

            modelBuilder.Entity<Book_Author>()
              .HasOne(b => b.Author)
              .WithMany(ba => ba.Book_Authors)
              .HasForeignKey(bi => bi.AuthorId);

            //define the relationship of book-user
            modelBuilder.Entity<UserBooks>()
             .HasOne(b => b.Book)
             .WithMany(bu => bu.UserBooks)
             .HasForeignKey(bi => bi.BookId);

            modelBuilder.Entity<UserBooks>()
              .HasOne(b => b.User)
              .WithMany(bu => bu.UserBooks)
              .HasForeignKey(bi => bi.UserId);

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Book_Author> Books_Authors { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<UserBooks> UserBooks { get; set; }

    }
}
