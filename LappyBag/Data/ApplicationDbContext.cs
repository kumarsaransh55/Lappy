using LappyBag.Models;
using Microsoft.EntityFrameworkCore;

namespace LappyBag.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Gaming", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Productivity", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Business", DisplayOrder = 3 }
                );
        }
    }
}
