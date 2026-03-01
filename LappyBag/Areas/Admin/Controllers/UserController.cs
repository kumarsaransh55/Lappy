using System.Collections.Generic;
using Lappy.DataAccess.Data;
using Lappy.DataAccess.Repository;
using Lappy.DataAccess.Repository.IRepository;
using Lappy.Models;
using Lappy.Models.ViewModels;
using Lappy.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Azure;

namespace LappyBag.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _dbContext;
        private readonly  RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(IUnitOfWork unitOfWork, ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> RoleManagement(string id)
        {
            var user = _unitOfWork.ApplicationUser.Get(u => u.Id == id);

            var roleStore = await _userManager.GetRolesAsync(user);
            string currentRole = roleStore.FirstOrDefault();

            var userObj = new RoleVM
            {
                Id = user.Id,
                Name = user.Name,
                Role = currentRole,
                CompanyId = user.CompanyId,
                RoleList = _roleManager.Roles.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Name,
                    Selected = u.Name == currentRole
                }),
                CompanyList = _unitOfWork.Company.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                    Selected = u.Id == user.CompanyId
                })
            };
                       
            return View(userObj);
        }
        [HttpPost]
        public async Task<IActionResult> RoleManagement(RoleVM roleVM)
        {
            // 1. Fetch user using UserManager (this makes it the "Tracked" instance)
            var user = await _userManager.FindByIdAsync(roleVM.Id);

            if (user == null)
            {
                return NotFound();
            }

            // 2. Get the current role (Identity way)
            var roles = await _userManager.GetRolesAsync(user);
            var oldRole = roles.FirstOrDefault();

            if (oldRole != roleVM.Role)
            {
                // 3. Perform Role Changes
                if (!string.IsNullOrEmpty(oldRole))
                {
                    await _userManager.RemoveFromRoleAsync(user, oldRole);
                }
                await _userManager.AddToRoleAsync(user, roleVM.Role);

                // 4. Update Properties on the SAME tracked object
                if (oldRole == SD.Role_Company)
                {
                    user.CompanyId = null;
                }
                if (roleVM.Role == SD.Role_Company)
                {
                    user.CompanyId = roleVM.CompanyId;
                }

                // 5. Final Save
                // Because 'user' is already being tracked by the context 
                // through the UserManager, we just need to save.
                _unitOfWork.Save();
            }

            return RedirectToAction("Index");
        }
        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var userList = from user in _dbContext.ApplicationUsers
                           join userRole in _dbContext.UserRoles on user.Id equals userRole.UserId
                           join role in _dbContext.Roles on userRole.RoleId equals role.Id
                           select new
                           {
                               user.Id,
                               user.Name,
                               user.Email,
                               user.PhoneNumber,
                               CompanyName = user.Company.Name,
                               Role = role.Name,
                               user.LockoutEnd
                           };

            return Json(new { data = userList });
        }

        [HttpPost]
        public IActionResult LockUnlock(string id)
        {
            var userinfo = _unitOfWork.ApplicationUser.Get(u => u.Id == id);
            if (userinfo != null)
            {
                if (userinfo.LockoutEnd != null && userinfo.LockoutEnd > DateTime.Now)
                {
                    userinfo.LockoutEnd = DateTime.Now;
                }
                else
                {
                    userinfo.LockoutEnd = DateTime.Now.AddYears(1000);
                }
            }
            else
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }
            _unitOfWork.ApplicationUser.Update(userinfo);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Operation Successful" });
        }

        #endregion
    }
}
