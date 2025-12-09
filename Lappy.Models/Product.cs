using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lappy.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Brand {  get; set; }
        [Required]
        [Display(Name = "Model Name")]
        public string ModelName { get; set; }
        [Required]
        [Display(Name = "Screen Size")]
        public string ScreenSize { get; set; }
        [Required]
        public string Colour{ get; set; }
        [Required]
        [Display(Name = "Hard Disk Size")]
        public string HardDiskSize{ get; set; }
        [Required]
        [Display(Name = "CPU Model")]
        public string CpuModel {  get; set; }
        [Required]
        [Display(Name = "RAM Memory Size")]
        public string RamMemory {  get; set; }
        [Required]
        [Display(Name = "Operating System")]
        public string OS {  get; set; }
        [Required]
        [Display(Name = "Special Features")]
        public string SpecialFeatures{ get; set; }
        [Required]
        [Display(Name = "Graphics Card")]
        public string GraphicsCard{ get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [Range(1,10000000)]
        [Display(Name = "List Price")]
        public double ListPrice {  get; set; }
        [Required]
        [Range(1, 100000000)]
        [Display(Name = "Price for 10-25 Quantity")]
        public double ListPrice10 { get; set; }
        [Required]
        [Range(1, 1000000000)]
        [Display(Name = "Price for 25-50 Quantity")]
        public double ListPrice25 { get; set; }
        [Required]
        [Range(1, 1000000000)]
        [Display(Name = "Price for >50 Quantity")]
        public double ListPrice100 { get; set; }
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        public string ImageUrl {  get; set; }
    }
}
