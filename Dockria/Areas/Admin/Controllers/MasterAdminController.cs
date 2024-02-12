using Dockria.Data;
using Domain.Model;
using Domain.Model.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Domain.Vaildation;
using DocumentFormat.OpenXml.InkML;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]

    public class MasterAdminController : Controller
    {
        // private readonly RoleManager<ApplicationUser> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private EmailSettings _emailSettings { get; }


        //   private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public MasterAdminController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<EmailSettings> emailSettings,
            //  RoleManager<ApplicationUser> roleManager,
            ApplicationDbContext context
          )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSettings = emailSettings.Value;
            //  _emailSender = emailSender;
            _context = context;
            //  _roleManager = roleManager;
        }

        private string GenerateRandomCode()
        {
            // Implement your logic to generate a unique random code
            // For example, you can use Guid.NewGuid() to generate a unique identifier
            return Guid.NewGuid().ToString();
        }

        [HttpGet]
        public IActionResult Home(string id)
        {
            // Use the id parameter as needed in your Home action
            ViewBag.RandomString = id;

            // If id is not provided or is empty, generate a new random code
            if (string.IsNullOrEmpty(id))
            {
                string randomCode = GenerateRandomCode();
                ViewBag.RandomString = randomCode;
                return RedirectToAction("Home", new { id = randomCode });
            }

            return View();
        }
        [HttpGet]
        public IActionResult Index(string id)
        {
            ViewBag.RandomString = id;

            if (string.IsNullOrEmpty(id))
            {
                string randomCode = GenerateRandomCode();
                ViewBag.RandomString = randomCode;
                return RedirectToAction("Index", new { id = randomCode });
            }

            var userList = _context.ApplicationUsers.ToList();
            var roles = _context.Roles.ToList();
            var userRole = _context.UserRoles.ToList();
            var usersToRemove = new List<ApplicationUser>();

            foreach (var user in userList)
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;

                if (user.Role == SD.Role_User || user.Role == SD.Role_Company_Admin)
                {
                    usersToRemove.Add(user);
                }
            }

            foreach (var userToRemove in usersToRemove)
            {
                userList.Remove(userToRemove);
            }

            ViewBag.CountUsers = userList.Count();

            return View(userList);
        }
        [HttpGet]
        public IActionResult List()
        {
            var admins = _context.ApplicationUsers.ToList();

            return View(admins);
        }

        [HttpGet]
        public IActionResult AdminList()
        {
            var admins = _context.ApplicationUsers.Where(u => u.Role == "Admin").ToList();

            return PartialView("_AdminList", admins);
        }

        [HttpGet]
        public IActionResult Create(string code)
        {
            // If code is not provided or is empty, generate a new random code
            if (string.IsNullOrEmpty(code))
            {
                // Generate a new random code
                string randomCode = GenerateRandomCode();

                // Include the random code in the ViewBag
                ViewBag.RandomCode = randomCode;

                // Redirect to the same action with the generated random code in the URL
                return RedirectToAction("Create", new { code = randomCode });
            }

            // Use the code parameter as needed in your Create action
            ViewBag.RandomString = code;

            var userList = _context.ApplicationUsers.ToList();
            var roles = _context.Roles.ToList();
            var userRole = _context.UserRoles.ToList();

            userList.RemoveAll(user =>
            {
                var roleId = userRole.FirstOrDefault(u => u.UserId == user.Id)?.RoleId;
                var roleName = roles.FirstOrDefault(r => r.Id == roleId)?.Name;

                return roleName == SD.Role_User || roleName == SD.Role_Company_Admin;
            });

            return View(userList);
        }


        public async Task Execute(string email, string subject, string msg)
        {
            try
            {
                string toEmail = string.IsNullOrEmpty(email) ? _emailSettings.ToEmail : email;
                MailMessage message = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "rajputshilpa710@gmail.com")
                };
                message.To.Add(new MailAddress(toEmail));
                message.CC.Add(new MailAddress(_emailSettings.CcEmail));
                message.Subject = "Dockria : " + subject;
                message.Body = msg;
                message.IsBodyHtml = true;
                message.Priority = MailPriority.High;
                using (SmtpClient smtp = new SmtpClient(_emailSettings.PrimaryDomain, _emailSettings.PrimaryPort))
                {
                    smtp.Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(message);
                }
            }
            catch (Exception ex)
            {

                string str = ex.Message;
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ApplicationUser model)
        {
            bool emailExists = await _context.ApplicationUsers.AnyAsync(c => c.EmailAddress == model.EmailAddress);
            if (emailExists)
            {
                ModelState.AddModelError("AdminEmail", "Email already exists.");

            }

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.EmailAddress, // Use the email address as the username
                    FullName = model.FullName,
                    EmailAddress = model.EmailAddress,
                    ConfirmEmailAddress = model.ConfirmEmailAddress,
                    PhoneNumber = model.PhoneNumber,
                };

                var password = SD.CreatePassword();

                var result = await _userManager.CreateAsync(user, user.PasswordHash = password);

                await Execute(user.EmailAddress, "Your User Creadential details", "Your User Name : " + user.EmailAddress + "\nYour password : " + password);

                if (result.Succeeded)
                {
                    // Add the user to the specified role
                    await _userManager.AddToRoleAsync(user, "Admin");

                    return Json(new { success = true });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);

            return Json(new { success = false, errors });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == user.Id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            var viewModel = new ApplicationUser
            {
                Id = user.Id,
                FullName = applicationUser.FullName,
                UserName = user.UserName,
                EmailAddress = applicationUser.EmailAddress,
                ConfirmEmailAddress = applicationUser.ConfirmEmailAddress,
                PhoneNumber = user.PhoneNumber
            };

            return PartialView("_AdminForm", viewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] ApplicationUser model)
        {
            try

            {
                if (ModelState.IsValid)
                {
                    var applicationUser = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == model.Id);
                    if (applicationUser == null)
                    {
                        return NotFound();
                    }

                    applicationUser.FullName = model.FullName;
                    applicationUser.ConfirmEmailAddress = model.ConfirmEmailAddress;
                    applicationUser.EmailAddress = model.EmailAddress;
                    applicationUser.PhoneNumber = model.PhoneNumber;

                    var result = await _userManager.UpdateAsync(applicationUser);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true });
                }

                // If ModelState is not valid, return BadRequest with ModelState errors
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                // Log the exception for further investigation
                Console.WriteLine(ex.Message);
                return StatusCode(500, "An error occurred while processing the request.");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Getdetails(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var applicationUser = await _context.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == user.Id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            var viewModel = new ApplicationUser
            {
                Id = user.Id,
                FullName = applicationUser.FullName,
                UserName = user.UserName,
                EmailAddress = applicationUser.EmailAddress,
                ConfirmEmailAddress = applicationUser.ConfirmEmailAddress,
                PhoneNumber = user.PhoneNumber
            };

            return PartialView("_AdminForm", viewModel);

        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Perform additional cleanup or actions here, if needed
            // For example, delete related data from other tables

            // Delete the user from the custom ApplicationUsers table
            var applicationUser = await _context.ApplicationUsers.FindAsync(id);
            if (applicationUser != null)
            {
                _context.ApplicationUsers.Remove(applicationUser);
                await _context.SaveChangesAsync();
            }

            // Delete the user from the Identity ASP.NET Core table
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Json(new { success = true });
        }


        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var auditLogs = _context.AuditLogs.ToList();
            var auditLogViewModels = auditLogs.Select(log => new AuditLog
            {
                Date = log.Date,
                Time = log.Time,
                Username = log.Username,
                Fullname = log.Fullname,
                BrowserName = log.BrowserName,
                IPAddress = log.IPAddress,
                // Action = log.Action,
                RecordType = log.RecordType,
                // OldData = log.OldData,
                NewData = log.NewData,
                // Description = log.Description
            }).ToList();

            return Json(new { status = true, data = auditLogViewModels });
        }

        [HttpGet]
        public IActionResult GetAuditLogs()
        {
            var currentTime = DateTime.Now;
            var tenSecondsAgo = currentTime.AddSeconds(-10);

            var auditLogs = _context.AuditLogs
                .Where(log => log.Date >=
                tenSecondsAgo && log.Date <= currentTime)
                .OrderByDescending(log => log.Time)
                .ToList();

            return Json(auditLogs);
        }
        #endregion
    }
}
