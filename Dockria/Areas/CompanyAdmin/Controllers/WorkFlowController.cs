using Dockria.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebApp.Areas.CompanyAdmin.Controllers
{
    [Area("CompanyAdmin")]
    public class WorkFlowController : Controller
    {
        private ApplicationDbContext _context;

        public WorkFlowController(ApplicationDbContext context)
        {
            _context = context;
        }
        private string GenerateRandomCode()
        {
            // Implement your logic to generate a unique random code
            // For example, you can use Guid.NewGuid() to generate a unique identifier
            return Guid.NewGuid().ToString();
        }


        [HttpGet]
        public IActionResult Index(string id)
        {
            // Use the id parameter as needed in your CompanyAdmin action
            ViewBag.RandomString = id;

            // If id is not provided or is empty, generate a new random code
            if (string.IsNullOrEmpty(id))
            {
                string randomCode = GenerateRandomCode();
                ViewBag.RandomString = randomCode;
                return RedirectToAction("Index", new { id = randomCode });
            }

            return View();
        }



        [HttpGet]
        public IActionResult ShowUserList()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var users = _context.CompanyAdmin.Where(c => c.AspId == claim.Value).ToList();
                var userList = new List<object>();

                foreach (var user in users)
                {
                    var userGroupList = _context.UserGroups.Where(x => x.CompanyAdminId == user.Id).ToList();

                    var userGroups = userGroupList.Select(u => new
                    {
                        Value = u.UserGroupId,
                        Text = u.UserGroupName,
                    }).ToList();

                    userList.AddRange(userGroups);
                }
                var userData = new { userlist = userList };
                return Json(userData);
            }

            return Json(new { success = "false", data = new List<object>() });
        }
    }
}