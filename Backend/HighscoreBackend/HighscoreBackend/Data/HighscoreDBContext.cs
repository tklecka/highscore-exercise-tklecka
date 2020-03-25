using HighscoreBackend.Model;
using Microsoft.EntityFrameworkCore;

namespace HighscoreBackend.Data
{
    public class HighscoreDBContext : DbContext
    {
        public DbSet<Highscore> Highscore { get; set; }

        public HighscoreDBContext(DbContextOptions<HighscoreDBContext> options) : base(options) { }
    }
}
