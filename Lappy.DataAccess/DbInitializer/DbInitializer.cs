using Lappy.DataAccess.Data;
using Lappy.Models;
using Lappy.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lappy.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public DbInitializer(UserManager<IdentityUser> userManager,
                             RoleManager<IdentityRole> roleManager,
                             ApplicationDbContext dbContext,
                             IConfiguration configuration)
        {

            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public void InitializeAsync()
        {
            try
            {
                if (_dbContext.Database.GetPendingMigrations().Count() > 0)
                {
                    _dbContext.Database.Migrate();
                }
            }
            catch (Exception ex)
            {}

            if ( !_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = _configuration["Admin:userId"],
                    Email = _configuration["Admin:userId"],
                    Name = "Saransh Kumar",
                    PhoneNumber = "8700661336",
                    StreetAddress = "House- 34/3, Pearl Enclave, Patel Nagar",
                    State = "Delhi",
                    PinCode = "110034",
                    City = "New Delhi"
                }, _configuration["Admin:pwd"]).GetAwaiter().GetResult();

                ApplicationUser user = _dbContext.ApplicationUsers.FirstOrDefault(u => u.Email == _configuration["Admin:userId"]);
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
            }
            return;
        }
    }
}