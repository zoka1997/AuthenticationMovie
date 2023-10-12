using Microsoft.EntityFrameworkCore;
using MoviesApplication.Models.MoviesModels;

namespace MoviesApplication.DatabaseContext.MoviesContext
{
    public class MoviesDbContext : DbContext
    {
        public MoviesDbContext(DbContextOptions<MoviesDbContext> options)
            : base(options) 
        {

        }

        public DbSet<MoviesShow> MoviesShows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MoviesShow>()
                .Property(m => m.Rating)
                .HasColumnType("decimal(12, 2)"); // 5 total digits, 2 decimal places
        }
    }
}
