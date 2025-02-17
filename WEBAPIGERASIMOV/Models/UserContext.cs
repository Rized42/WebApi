using Microsoft.EntityFrameworkCore;

namespace WEBAPIGERASIMOV.Models
{
    public class UserContext: DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options) 
        { 
        }

        public DbSet<UserItem> Users { get; set; } = null!;
    }
}
