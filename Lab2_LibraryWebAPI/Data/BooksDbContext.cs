using Microsoft.EntityFrameworkCore;

namespace Lab2_LibraryWebAPI.Data
{
    public class BooksDbContext : DbContext
    {
        public BooksDbContext(DbContextOptions<BooksDbContext> options) : base(options)
        {
        }
    }
}
