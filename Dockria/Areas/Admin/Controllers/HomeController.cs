using Dockria.Data;
using Dockria.Models;
using Domain.Model;
using Microsoft.AspNetCore.Authorization; // Add this using statement
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

      
        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                bool isLocked = false;
                var userInDb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == claim.Value);
                if (User.IsInRole(SD.Role_Company_Admin))
                {
                    // Redirect to Master Admin home
                    return Redirect("/CompanyAdmin/CompanyAdmin");
                }
                else
                {
                    // Redirect to Company Create User page
                    return Redirect("/Admin/MasterAdmin/Home");
                }
            }
            else
            {
                return Redirect("/Identity/Account/Login");
            }
        }

        [Authorize(Roles = "CompanyAdmin")] // This action is also protected by authorization
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
