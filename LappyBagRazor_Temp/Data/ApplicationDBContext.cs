using LappyBagRazor_Temp.Models;
using Microsoft.EntityFrameworkCore;

namespace LappyBagRazor_Temp.Data
{
    public class ApplicationDBContext:DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options):base(options)
        {}
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
