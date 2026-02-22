// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Lappy.DataAccess.Repository.IRepository;
using Lappy.Models;
using Lappy.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace LappyBag.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> RoleManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _RoleManager = RoleManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            
            public string? Role {get; set;}

            [ValidateNever]
            [Display(Name = "Select a Role")]
            public IEnumerable<SelectListItem> RoleList { get; set; }

            public int? CompanyId { get; set; }

            [ValidateNever]
            [Display(Name = "Select a Role")]
            public IEnumerable<SelectListItem> CompanyList { get; set; }

            [Required]
            public string? Name { get; set; }
            public string? PhoneNumber { get; set; }
            public string? StreetAddress { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? PinCode { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            Input = new InputModel();
            Input.RoleList = _RoleManager.Roles.Select(x => x.Name).Select(i=> new SelectListItem
            {
                Text = i,
                Value = i
            }).ToList();
            Input.CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem {
                Text = i.Name,
                Value = i.Id.ToString()
            }).ToList();
            
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        private string getWelcomeTemplate(string name)
        {
            return $@"
    <div style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 30px;'>
        <div style='max-width: 600px; margin: auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; shadow: 0 4px 10px rgba(0,0,0,0.1);'>
            <!-- Header -->
            <div style='background-color: #375a7f; padding: 20px; text-align: center;'>
                <h1 style='color: #ffffff; margin: 0; font-size: 24px;'>LAPPY<span style='color: #0dcaf0;'>TECH</span></h1>
            </div>
            
            <!-- Body -->
            <div style='padding: 30px; color: #333333; line-height: 1.6;'>
                <h2 style='color: #375a7f;'>Welcome to the Platform, {name}!</h2>
                <p>Thank you for registering with <strong>LappyTech Procurement</strong>. Your account is now active and ready for use.</p>
                
                <p>As a registered member, you now have access to:</p>
                <ul style='padding-left: 20px;'>
                    <li>Real-time inventory of enterprise-grade hardware.</li>
                    <li>Tiered pricing based on procurement volume.</li>
                    <li>Order tracking and history dashboard.</li>
                </ul>

                <div style='text-align: center; margin: 30px 0;'>
                    <a href='https://lappytech.azurewebsites.net/' 
                       style='background-color: #0dcaf0; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                       Explore Hardware Inventory
                    </a>
                </div>

                <p>If you have any questions regarding B2B pricing or corporate credit lines, please contact our support team.</p>
                
                <p>Best Regards,<br/>The LappyTech Team</p>
            </div>

            <!-- Footer -->
            <div style='background-color: #eeeeee; padding: 15px; text-align: center; font-size: 12px; color: #777777;'>
                &copy; {DateTime.Now.Year} LappyTech IT Solutions. All rights reserved.
            </div>
        </div>
    </div>";
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (Input.Role == SD.Role_Company && Input.CompanyId == null)
            {
                // Add error manually
                ModelState.AddModelError("Input.CompanyId", "Please select your Company.");
            }
            else
            {
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                if (ModelState.IsValid)
                {
                    var user = CreateUser();

                    await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                    user.Name = Input.Name;
                    user.PhoneNumber = Input.PhoneNumber;
                    user.StreetAddress = Input.StreetAddress;
                    user.City = Input.City;
                    user.State = Input.State;
                    user.PinCode = Input.PinCode;
                    if (Input.Role == SD.Role_Company)
                        user.CompanyId = Input.CompanyId;

                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        if (!string.IsNullOrEmpty(Input.Role))
                        {
                            await _userManager.AddToRoleAsync(user, Input.Role);
                        }
                        else
                        {
                            await _userManager.AddToRoleAsync(user, SD.Role_Customer);
                        }

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        //   $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            if (!(User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee)))
                            {
                                var subject = "Welcome to LappyTech - Enterprise IT Procurement";

                                var welcomeMessage = $@"{getWelcomeTemplate(Input.Name)}";

                                await _emailSender.SendEmailAsync(Input.Email, subject, welcomeMessage);

                                await _signInManager.SignInAsync(user, isPersistent: false);
                            }
                            else
                            {
                                TempData["success"] = "New User Created Successfully";
                            }
                            return LocalRedirect(returnUrl);
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        if (error.Code == "DuplicateUserName")
                        {
                            // Replace the default message with your own
                            ModelState.AddModelError(string.Empty, $"Email '{Input.Email}' is already taken.");
                        }
                        else
                        {
                            // For all other errors (like weak password), keep the default
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            Input.RoleList = _RoleManager.Roles.Select(x => x.Name).Select(i => new SelectListItem
            {
                Text = i,
                Value = i
            }).ToList();
            Input.CompanyList = _unitOfWork.Company.GetAll().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            }).ToList();

            return Page();
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
