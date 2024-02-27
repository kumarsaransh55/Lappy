using Lappy.DataAccess.Data;
using Lappy.DataAccess.Repository.IRepository;
using Lappy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lappy.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }
        public void Update(Product obj)
        {
            {
                //var _product= _db.Products.Where(u=>u.Id==obj.Id).FirstOrDefault();
                //if (_product!=null)
                //{
                //    _product.Colour = obj.Colour;
                //    _product.CpuModel = obj.CpuModel;
                //    _product.GraphicsCard=obj.GraphicsCard;
                //    _product.Brand = obj.Brand;
                //    _product.RamMemory = obj.RamMemory;
                //    _product.ListPrice = obj.ListPrice;
                //    _product.ListPrice100 = obj.ListPrice100;
                //    _product.ListPrice25 = obj.ListPrice25;
                //    _product.ListPrice10 = obj.ListPrice10;
                //    _product.CategoryId = obj.CategoryId;
                //    _product.Description = obj.Description;
                //    _product.HardDiskSize = obj.HardDiskSize;
                //    _product.ModelName = obj.ModelName;
                //    _product.OS = obj.OS;
                //    _product.ScreenSize = obj.ScreenSize;
                //    _product.SpecialFeatures = obj.SpecialFeatures;
                //    if(obj.ImageUrl!= null)
                //    {
                //        _product.ImageUrl = obj.ImageUrl;
                //    }
            }
            _db.Products.Update(obj);
        }
    }
}
