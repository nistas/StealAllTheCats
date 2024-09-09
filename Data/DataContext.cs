using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using StealAllTheCats.Entities;

namespace StealAllTheCats.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<CatTag>()
                .HasKey(catTag => new { catTag.CatId, catTag.TagId });
           

        }
        public DbSet<Cat> Cats { get; set; }
        public DbSet<Tag> Tags { get; set; }

        public DbSet<CatTag> CatTag { get; set; }

    }
}
