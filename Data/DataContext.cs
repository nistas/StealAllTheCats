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
            //modelBuilder.Entity<Cat>()
            //    .HasMany(e => e.Tags)
            //    .WithMany(e => e.Cats);


            //modelBuilder.Entity<Tag>()
            //    .HasMany(e => e.Cats)
            //    .WithMany(e => e.Tags);

            //modelBuilder.Entity<Cat>()
            //    .HasMany(e => e.Tags)
            //    .WithMany(e => e.Cats)
            //    .UsingEntity(
            //        "CatTag",
            //        l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagsId").HasPrincipalKey(nameof(Tag.Id)),
            //        r => r.HasOne(typeof(Cat)).WithMany().HasForeignKey("CatsId").HasPrincipalKey(nameof(Cat.Id)),
            //        j => j.HasKey("CatsId", "TagsId"));


        }
        public DbSet<Cat> Cats { get; set; }
        public DbSet<Tag> Tags { get; set; }

        //public DbSet<CatTag> CatTag { get; set; }

    }
}
