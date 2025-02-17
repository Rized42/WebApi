using Microsoft.EntityFrameworkCore;

namespace WEBAPIGERASIMOV.Models
{
    public class BookContext: DbContext
    {
        public BookContext(DbContextOptions<BookContext> options): base(options)  { }

        public DbSet<BookItem> Books { get; set; } = null!;
    }
}
