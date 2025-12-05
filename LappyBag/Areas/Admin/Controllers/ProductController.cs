using System.Collections.Generic;
using Lappy.DataAccess.Repository;
using Lappy.DataAccess.Repository.IRepository;
using Lappy.Models;
using Lappy.Models.ViewModels;
using Lappy.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LappyBag.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> obj= _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();
            return View(obj);
        }
        public IActionResult Upsert(int? id)
         {
            IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() });
            //ViewBag.categoryListVb=categoryList;
            // We can also transfer data from controller to view which is not present in the model itself by using viewdata(Little hard) Like by
            // ViewData["categoryList"] and using in view by casting it as asp-items="@(ViewData[categoryList] as IEnumerable<SelectListItem>)"
            Product product = new Product();
            if (id != null && id!=0)
            {
                product= _unitOfWork.Product.Get(u=>u.Id==id);
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
                    _unitOfWork.Product.Add(productVm.Product);
                    TempData["successmsg"] = "Product Created Successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(productVm.Product);
                    TempData["successmsg"] = "Product Updated Successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            last:
            productVm.CategoryListVb= _unitOfWork.Category.GetAll().Select(u => new SelectListItem { Text = u.Name, Value = u.Id.ToString() });
            return View(productVm);
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Product objtoBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (objtoBeDeleted != null)
            {
                string imagePath = Path.Combine(_webHostEnvironment.WebRootPath,objtoBeDeleted.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _unitOfWork.Product.Remove(objtoBeDeleted);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Delete Successful" });
            }
            return Json(new { success = false, message = "Error while Deleting" });
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> obj = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = obj });
        }
        #endregion
    }
}
