﻿// Licensed to the .NET Foundation under one or more agreements.
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
using Cqrs.Domain;
using Domain.Model;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace WebApp.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        //   private readonly IUnitOfWork _unitOfWork;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            //   IUnitOfWork unitOfWork,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            //  _unitOfWork = unitOfWork;
            _roleManager = roleManager;
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
            //[Required]
            //[EmailAddress]
            //[Display(Name = "Email")]
            //public string Email { get; set; }

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
            /// </summary>`
            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            // Add Customize Columns
            [Required]
            public string FullName { get; set; }
            [Required]
            [Display(Name ="Email Address")]
            public string EmailAddress { get; set; }    

            [Required]
            [Display(Name ="Confirm Email Address")]
            public string ConfirmEmailAddress { get; set; }

            [Required]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }
          //  public string Role { get; set; }

            // Roles
         //   public IEnumerable<SelectListItem> RoleList { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            //Input = new InputModel()
            //{
            //    RoleList = _roleManager.Roles.Where(r => r.Name != SD.Role_Admin).Select(r => r.Name).Select(rl => new SelectListItem
            //    {
            //        Text = rl,
            //        Value = rl
            //    })
            //};
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    FullName = Input.FullName,
                     EmailAddress= Input.EmailAddress,
                   

                 //  Email = Input.Email,
                    UserName = Input.EmailAddress,
                    ConfirmEmailAddress= Input.ConfirmEmailAddress,

                    
                    PhoneNumber = Input.PhoneNumber



                    //Role = Input.Role,
                    //  CreateTime = DateTime.Now
                };

                //  if (Input.Role == SD.Role_Company_Admin || Input.Role == SD.Role_User)
                //{
                // user.Expirytime = DateTime.Now.AddDays(30);
                // }

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");


                    //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    //var callbackUrl = Url.Page(
                    //    "/Account/ConfirmEmail",
                    //    pageHandler: null,
                    //    values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                    //    protocol: Request.Scheme);

                    //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                    //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (!await _roleManager.RoleExistsAsync(SD.Role_Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Role_Company_Admin))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_Company_Admin));
                    }
                    if (!await _roleManager.RoleExistsAsync(SD.Role_User))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(SD.Role_User));
                    }

                    // Admin Role Use this code ones for creating Admin User First Time.... 
                     await _userManager.AddToRoleAsync(user, SD.Role_Admin);

                    //if (Input.Role == null)
                    //{
                    //    await _userManager.AddToRoleAsync(user, SD.Role_User);
                    //    //  await _userManager.AddToRoleAsync(user, SD.Role_Admin);
                    //}
                    //else
                    //{
                    //    await _userManager.AddToRoleAsync(user, Input.Role);
                    //}



                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.EmailAddress  , returnUrl = returnUrl });
                    }
                    else
                    {
                        //if (user.Role == SD.Role_User || user.Role == null)
                        //{
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        //}
                        //else
                        //{
                        //    return RedirectToAction("Index", "Company", new { Area = "Admin" });
                        //}

                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            //Input = new InputModel()
            //{
            //    RoleList = _roleManager.Roles.Where(r => r.Name != SD.Role_Admin).Select(r => r.Name).Select(rl => new SelectListItem
            //    {
            //        Text = rl,
            //        Value = rl
            //    })
            //};

            // If we got this far, something failed, redisplay form
            return Page();
            //}

            //private IdentityUser CreateUser()
            //{
            //    try
            //    {
            //        return Activator.CreateInstance<IdentityUser>();
            //    }
            //    catch
            //    {
            //        throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
            //            $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
            //            $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            //    }
            //}

            //private IUserEmailStore<IdentityUser> GetEmailStore()
            //{
            //    if (!_userManager.SupportsUserEmail)
            //    {
            //        throw new NotSupportedException("The default UI requires a user store with email support.");
            //    }
            //    return (IUserEmailStore<IdentityUser>)_userStore;
            //}
        }
    }
}
