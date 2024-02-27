using Lappy.DataAccess.Repository;
using Lappy.DataAccess.Repository.IRepository;
using Lappy.Models;
using Lappy.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace LappyBag.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> obj= _db.Product.GetAll(includeProperties:"Category").ToList();
            return View(obj);
        }
        public IActionResult Upsert(int? id)
         {
            IEnumerable<SelectListItem> categoryList = _db.Category.GetAll().Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() });
            //ViewBag.categoryListVb=categoryList;
            // We can also transfer data from controller to view which is not present in the model itself by using viewdata(Little hard) Like by
            // ViewData["categoryList"] and using in view by casting it as asp-items="@(ViewData[categoryList] as IEnumerable<SelectListItem>)"
            Product product = new Product();
            if (id != null && id!=0)
            {
                product= _db.Product.Get(u=>u.Id==id);
            }
            ProductVM productvM = new ProductVM()
            {
                CategoryListVb = categoryList,
                Product = product
            };
            return View(productvM);
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM productVm, IFormFile? file)
        {
            if(file==null && productVm.Product.Id == 0)
            {
                ModelState.AddModelError("err", "Please Upload Image while Creating a Product");
                goto last;
            }
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename=Guid.NewGuid().ToString() +Path.GetExtension(file.FileName ) ; //Random name to image file
                    string productPath= Path.Combine(wwwRootPath,@"images/product");
                    if (!string.IsNullOrEmpty(productVm.Product.ImageUrl))
                    {
                        var oldImagePath= Path.Combine(wwwRootPath,productVm.Product.ImageUrl.Trim('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVm.Product.ImageUrl = @"\images\product\" + filename;
                }
                if (productVm.Product.Id == 0)
                {
                    _db.Product.Add(productVm.Product);
                    TempData["successmsg"] = "Product Created Successfully";
                }
                else
                {
                    _db.Product.Update(productVm.Product);
                    TempData["successmsg"] = "Product Updated Successfully";
                }
                _db.Save();
                return RedirectToAction("Index");
            }
            last:
            productVm.CategoryListVb= _db.Category.GetAll().Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() });
            return View(productVm);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product obj = _db.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(obj);
        }

        [HttpPost]
        public IActionResult Delete(Product? obj)
        {
            if (obj != null)
            {
                    _db.Product.Remove(obj);
                    _db.Save();
                    TempData["successmsg"] = "Product Deleted Successfully";
                    return RedirectToAction("Index");
             
            }
             return View();
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> obj = _db.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = obj });
        }
        #endregion
    }
}
