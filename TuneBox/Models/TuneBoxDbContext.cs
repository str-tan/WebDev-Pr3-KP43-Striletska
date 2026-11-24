using Microsoft.EntityFrameworkCore;

namespace TuneBox.Models
{
    public class TuneBoxDbContext : DbContext
    {
        public TuneBoxDbContext(DbContextOptions<TuneBoxDbContext> options)
            : base(options)
        {
        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Song> Songs { get; set; }
    }
}