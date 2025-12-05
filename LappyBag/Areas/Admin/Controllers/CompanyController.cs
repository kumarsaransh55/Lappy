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
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IEnumerable<Company> obj= _unitOfWork.Company.GetAll().ToList();
            return View(obj);
        }
        
        [HttpGet]
        public IActionResult Upsert(int? id)
        {
            Company company = _unitOfWork.Company.Get(u => u.Id == id) ?? new Company();
            return View(company);
        }
        
        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    _unitOfWork.Company.Add(company);
                    TempData["successmsg"] = "Company Created Successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                    TempData["successmsg"] = "Company Updated Successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            TempData["errormsg"] = "Unable to Update Company";
            return View(company);
        }

        #region API CALLS

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            Company objtoBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (objtoBeDeleted != null)
            {
                _unitOfWork.Company.Remove(objtoBeDeleted);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Delete Successful" });
            }
            return Json(new { success = false, message = "Error while Deleting" });
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> obj = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = obj });
        }
        #endregion
    }
}
