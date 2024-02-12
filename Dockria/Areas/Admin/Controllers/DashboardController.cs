using Application.Companies.Queries;
using Dockria.Data;
using Domain.Model.ViewModel;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using System.Text;
using Domain.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;
        private EmailSettings _emailSettings { get; }

        public DashboardController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment,
            IOptions<EmailSettings> emailSettings, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _emailSettings = emailSettings.Value;
            _userManager = userManager;
        }

        public IActionResult Index(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                string randomCode = GenerateRandomCode();

                ViewBag.RandomCode = randomCode;

                return RedirectToAction("Index", new { code = randomCode });
            }
            ViewBag.RandomString = code;

            var model = _context.CompanyAdmin.ToList();
            ViewBag.CountCompany = _context.CompanyAdmin.Count();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CompaniesList()
        {
            var companies = _context.CompanyAdmin.ToList();
            ViewBag.CountCompany = _context.CompanyAdmin.Count();
            return PartialView("_companyAdminListPartialView", companies);
        }


        private string GenerateRandomCode()
        {
            return Guid.NewGuid().ToString();
        }

        [HttpGet]
        public IActionResult Details(string code)
        {
            ViewBag.RandomString = code;

            if (string.IsNullOrEmpty(code))
            {
                ViewBag.RandomString = GenerateRandomCode();
                return RedirectToAction("Details", new { code = ViewBag.RandomString });
            }

            var list = _context.CompanyAdmin.ToList();
            return View(list);
        }


        [HttpGet]
        public JsonResult UpdateListData()
        {
            var subscriptionList = _context.SmgViewModels
                .Select(x => new { Value = x.Id.ToString(), Text = x.SmgName })
                .ToList();

            var paymentData = new { PayList = subscriptionList };
            return Json(paymentData);
        }


        public async Task Execute(string email, string subject, string msg)
        {
            try
            {
                string toEmail = string.IsNullOrEmpty(email) ? _emailSettings.ToEmail : email;

                var currentUser = await _context.CompanyAdmin.FirstOrDefaultAsync(c => c.AdminEmail == email);

                MailMessage message = new MailMessage()
                {
                    From = new MailAddress(_emailSettings.UsernameEmail, "amandeepsoftiatric@gmail.com")
                };
                message.To.Add(new MailAddress(toEmail));
                message.CC.Add(new MailAddress(_emailSettings.CcEmail));

                string companyName = currentUser?.Name ?? "Unknown Company";
                string adminName = currentUser?.AdminName ?? "Unknown Admin";

                message.Subject = $"Welcome to Dockria - {companyName} - Your Company Admin is - {adminName}";
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
        public IActionResult DownloadInvoice(IFormFile pdfFile)
        {
            try
            {
                if (pdfFile != null && pdfFile.Length > 0)
                {
                    var targetDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "Admin", "Dashboard", "invoices");
                    Directory.CreateDirectory(targetDirectory);

                    var filePath = Path.Combine(targetDirectory, "invoice.pdf");


                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        pdfFile.CopyTo(fileStream);
                    }

                    return PhysicalFile(filePath, "application/pdf", "invoice.pdf");
                }

                return BadRequest("Invalid PDF file");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create(CompanyAdminUser user, List<IFormFile> files)
        {
            bool emailExists = await _context.CompanyAdmin.AnyAsync(c => c.AdminEmail == user.AdminEmail);
            bool userEmailExists = await _userManager.FindByEmailAsync(user.AdminEmail) != null;

            if (emailExists || userEmailExists)
            {
                ModelState.AddModelError("ReciveEmail", "Email already exists");
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                            .Select(error => error.ErrorMessage);
                return Json(new { success = false, message = errorMessages });
            }
            if (ModelState.IsValid)
            {
                var applicationUser = new ApplicationUser()
                {
                    FullName = user.Name,
                    EmailAddress = user.AdminEmail,
                    ConfirmEmailAddress = user.AdminEmail,
                    UserName = user.AdminEmail,
                    PhoneNumber = user.PhoneNumber
                };

                var password = SD.CreatePassword();
                //user.Password = password;

                var result = await _userManager.CreateAsync(applicationUser, password);

                if (result.Succeeded)
                {
                    try
                    {
                        await _userManager.AddToRoleAsync(applicationUser, SD.Role_Company_Admin);

                        user.AspId = applicationUser.Id;
                        user.ApplicationUser = applicationUser;

                        _context.CompanyAdmin.Add(user);
                        await _context.SaveChangesAsync();

                        string emailBody = $"Dear {user.AdminName},<br>We are pleased to welcome you to Dockria. Let us go\r\n paperless together. Below are your login details. Keep this email safe for future\r\n reference. DO NOT Share this email with anyone.<br>" +
                            $"<br><b>URL:</b> <a href='https://dockriadms.azurewebsites.net/'>https://dockriadms.azurewebsites.net/</a><br><b>Your User Name:</b> {user.AdminEmail}<br><b>Your password: </b>{password}<br><b>Number of User Licenses Assigned -<b><br><b> Disk Size in GB Assigned -<b>{user.StorageSpace}\r\n<br><b>Subscription Start Date -<b>{user.DateFrom}\r\n<br><b>Subscription End Date -<b>{user.DateTo}<br> If this email is sent to you by mistake, immediately email us at<br> support@dockria.com<br>Regards - Team Dockria";

                        await Execute(user.AdminEmail, "Your User Credential details", emailBody);

                        return Json(new { success = true });
                    }


                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        throw; 
                    }

                }
                else
                {
                    var errorMessages = result.Errors.Select(error => error.Description);
                    return Json(new { success = false, message = errorMessages });
                }
            }
            else
            {
                var modifiedErrors = new List<string>();

                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    if (!string.IsNullOrEmpty(error.ErrorMessage))
                    {
                        var modifiedErrorMessage = error.ErrorMessage.Replace("'", "\\'");
                        modifiedErrors.Add(modifiedErrorMessage);
                    }
                }

                return BadRequest(new { success = false, message = modifiedErrors });
            }
        }

        private string GetUniqueFileName(string fileName)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            string random = Path.GetRandomFileName().Replace(".", "").Substring(0, 8);
            return $"{timestamp}_{random}_{fileName}";
        }

        [HttpGet]
        public IActionResult EditFormPartial(int id)
        {
            var admin = _context.CompanyAdmin.Find(id);
            var smgId = admin.SubscriptionList;



            var selectedSmg = _context.SmgViewModels.FirstOrDefault(s => s.Id == id);
            var subscriptionList = _context.SmgViewModels.ToList();

            var jsonData = new
            {
                Admin = admin,
                //  SelectedSmgId = selectedSmg != null ? selectedSmg.Id : null,
                SubscriptionList = subscriptionList.Select(s => new { value = s.Id, text = s.SmgName })
            };

            return Json(jsonData);
        }



        [HttpPost]
        public async Task<IActionResult> EditFormPartial(CompanyAdminUser user)
        {
            if (user == null)
            {
                return BadRequest("Invalid data");
            }

            var companyAdmin = _context.CompanyAdmin.Find(user.Id);

            if (companyAdmin == null)
            {
                return NotFound();
            }

            var aspId = _context.ApplicationUsers.Where(c => c.Id == companyAdmin.AspId).FirstOrDefault();
            if (aspId != null)
            {
                if (user.Name != null && user.PhoneNumber != null)
                {
                    aspId.FullName = user.Name;
                    aspId.PhoneNumber = user.PhoneNumber;
                }

            }

            // Update the properties of the existing smg object
            companyAdmin.AdminName = user.AdminName;
            companyAdmin.Name = user.Name;
            companyAdmin.AdminEmail = user.AdminEmail;
            companyAdmin.Address = user.Address;
            companyAdmin.AdminDesignation = user.AdminDesignation;
            companyAdmin.OfficialEmail = user.OfficialEmail;
            companyAdmin.AdminPhoneNumber = user.AdminPhoneNumber;
            companyAdmin.PhoneNumber = user.PhoneNumber;
            companyAdmin.RegistrationNumber = user.RegistrationNumber;
            companyAdmin.SubscriptionName = user.SubscriptionName;
            companyAdmin.PinNumber = user.PinNumber;
            companyAdmin.FileNames = user.FileNames;
            companyAdmin.StorageSpace = user.StorageSpace;
            companyAdmin.InvoiceMail = user.InvoiceMail;
            companyAdmin.ReciveEmail = user.ReciveEmail;
            companyAdmin.DateFrom = user.DateFrom;
            companyAdmin.DateTo = user.DateTo;


            _context.CompanyAdmin.Update(companyAdmin);
            _context.SaveChanges();

            var response = new
            {
                CompanyAdmin = companyAdmin
            };

            return Ok(response);
        }


        [HttpGet]
        public JsonResult ShowData()
        {
            var list = _context.SmgViewModels.Select(x => new { Value = x.Id.ToString(), Text = x.SmgName }).ToList();

            var paymentData = new { PayList = list };

            //if (paymentData != null)
            //{
            return Json(paymentData);
            //}
            //else { return Json(new { success = false }); }
        }


        [HttpGet]
        public JsonResult GetCompanyUser(int id)
        {
            var admin = _context.CompanyAdmin
                .FirstOrDefault(c => c.Id == id);

            if (admin == null)
            {
                return Json(new { error = "Admin not found." });
            }

            var subscriptionNames = _context.SmgViewModels
                .Select(s => s.SmgName)
                .ToList();

            var responseData = new
            {
                data = new
                {
                    adminName = admin.AdminName,
                    id = admin.Id,
                    name = admin.Name,
                    adminEmail = admin.AdminEmail,
                    address = admin.Address,
                    adminDesignation = admin.AdminDesignation,
                    officialEmail = admin.OfficialEmail,
                    adminPhoneNumber = admin.AdminPhoneNumber,
                    phoneNumber = admin.PhoneNumber,
                    registrationNumber = admin.RegistrationNumber,
                    pinNumber = admin.PinNumber,
                    storageSpace = admin.StorageSpace,
                    invoiceMail = admin.InvoiceMail,
                    reciveEmail = admin.ReciveEmail,
                    dateFrom = admin.DateFrom,
                    dateTo = admin.DateTo,

                    subscriptionName = admin.SubscriptionName,
                    subscriptionNames = subscriptionNames
                }
            };

            return Json(responseData);
        }

        [HttpPost]
        public IActionResult UploadFile(List<IFormFile> files)
        {
            // Check file limit
            if (files.Count > 5)
            {
                return BadRequest("Maximum file upload limit is 5.");
            }
            return Ok("Files uploaded successfully!");
        }



        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.CompanyAdmin.FindAsync(id);
            if (user == null) return Json(new { success = false, message = "Something Wrong while delete data" });

            var aspId = _context.ApplicationUsers.Where(c => c.Id == user.AspId).FirstOrDefault();
            if (aspId != null)
            {
                _context.ApplicationUsers.Remove(aspId);
            }
            _context.Remove(user);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Company Admin Data Deleted Successfully..." });
        }
    }
}