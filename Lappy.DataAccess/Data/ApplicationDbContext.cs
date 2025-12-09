using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using Lappy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Lappy.DataAccess.Data
{
    public class ApplicationDbContext:IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers{ get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Gaming", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Productivity", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Business", DisplayOrder = 3 }
                );
            modelBuilder.Entity<Product>().HasData(new Product { Id=1,Brand="ASUS",ModelName= "ASUS Vivobook 15",ScreenSize= "15.6 Inches",Colour= "Quiet Blue",HardDiskSize= 
              "512 GB",CpuModel= "Core i3",RamMemory= "8 GB",OS= "Windows 11 Home",
              SpecialFeatures= "Backlit Keyboard, Anti Glare Coating",GraphicsCard= "Integrated",Description= "Processor: Intel Core i3-1215U Processor 1.2 GHz (10M Cache, up to 4.4 GHz, 6 cores)\r\nMemory: 8GB DDR4 RAM on board | Storage: 512GB M.2 NVMe PCIe 3.0 SSD\r\nDisplay: 15.6-inch, FHD (1920 x 1080) 16:9 aspect ratio, 250nits, 60Hz refresh rate, Anti-glare display, 45% NTSC\r\nGraphics: Integrated Intel UHD Graphics\r\nOperating System: Pre-installed Windows 11 Home with Lifetime Validity | Software Included: Pre-installed Office Home and Student with Lifetime Validity | McAfee (1 Year)\r\nDesign: 1.99 ~ 1.99 cm | 1.70 kg | Thin and Light Laptop | 42WHrs, 3S1P, 3-cell Li-ion | Up to 6 hours battery life ;Note: Battery life depends on conditions of usage\r\nKeyboard: Backlit Chiclet Keyboard | 1.4mm Key Travel",ListPrice=40990,ListPrice10=39000,
                ListPrice25=37000,ListPrice100=35000, CategoryId=2,ImageUrl=""
            });
        }
    }
}
