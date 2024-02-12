// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Middleware;
using Dockria.Data;
using DocumentFormat.OpenXml.InkML;
using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebApp.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;
        private readonly ApplicationDbContext _context;

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger,ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _logger = logger;
			_context= context;
        }


            public async Task<IActionResult> OnPost(string returnUrl = null)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Logout ApplicationUser
                var applicationUser = await _context.ApplicationUsers.FindAsync(userId);
                if (applicationUser != null)
                {
                    applicationUser.LastLoginDate = DateTime.Now;
                    _context.ApplicationUsers.Update(applicationUser);
                    await _context.SaveChangesAsync();
                }

                // Logout CompanyAdmin
                if (int.TryParse(userId, out int companyId))
                {
                    var companyAdmin = await _context.CompanyAdmin.FindAsync(companyId);
                    if (companyAdmin != null)
                    {
                       // companyAdmin.LastLoginDate = DateTime.Now;
                        _context.CompanyAdmin.Update(companyAdmin);
                        await _context.SaveChangesAsync();
                    }
                }

			// Log the audit information
			var log = new AuditLog
			{
				//Action = $"HTTP {HttpContext.Request.Method} request",
				Username = User.Identity.Name,
                BrowserName = HttpContext.Request.Headers["User-Agent"].ToString(),
                IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                Time = DateTime.Now.TimeOfDay, // Save only the time part
                Date = DateTime.Now.Date, // Save only the date part
                RecordType = "Logout",
                //Description= "Logout"
            };

			_context.AuditLogs.Add(log);
			await _context.SaveChangesAsync();

			await _signInManager.SignOutAsync();
                _logger.LogInformation("User logged out.");

                return RedirectToAction("Login", "Account");
            }


        //public async Task<IActionResult> OnPost(string returnUrl = null)
        //{
        //          var Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //          var user = new ApplicationUser();
        //          user = _context.ApplicationUsers.Find(Id)
        //	;
        //          user.LastLoginDate = DateTime.Now;
        //          _context.ApplicationUsers.Update(user);
        //          _context.SaveChanges();
        //          await _signInManager.SignOutAsync();
        //	_logger.LogInformation("User logged out.");

        //	return RedirectToAction("Login", "Account");
        //}


    }
}
