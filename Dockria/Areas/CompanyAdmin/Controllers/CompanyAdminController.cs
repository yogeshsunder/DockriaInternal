using ClosedXML.Excel;
using Dockria.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Security.Claims;

namespace WebApp.Areas.CompanyAdmin.Controllers
{
    [Area("CompanyAdmin")]
    public class CompanyAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public CompanyAdminController(ApplicationDbContext context, IEmailSender emailSender,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
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
        public IActionResult BulkUserAddition()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BulkUserAddition(IFormFile formFile)
        {
            if (formFile == null || formFile.Length <= 0)
            {
                // Handle empty file scenario
                return BadRequest("No file uploaded.");
            }

            try
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null)
                {
                    return Json(new { success = false, errorMessage = "User not found." });
                }
                var aspUser = await _userManager.FindByIdAsync(claim.Value);
                if (aspUser == null)
                {
                    return Json(new { success = false, errorMessage = "User not found." });
                }

                using (var stream = new MemoryStream())
                {
                    await formFile.CopyToAsync(stream);
                    using (var spreadsheetDocument = SpreadsheetDocument.Open(stream, false))
                    {
                        var sheet = spreadsheetDocument.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                        var worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(sheet.Id);
                        var rows = worksheetPart.Worksheet.Descendants<Row>();

                        var users = new List<CompanyUsers>();

                        // Create lists to collect success and error messages
                        var successMessages = new List<string>();
                        var errorMessages = new List<string>();

                        // Assuming data starts from row 2 (excluding header row)
                        foreach (var row in rows.Skip(1))
                        {
                            var cells = row.Elements<Cell>().ToList();

                            // Check if the cells in the row have non-empty data
                            bool hasData = cells.Any(cell => !string.IsNullOrEmpty(SD.GetCellValue(spreadsheetDocument, cell)));
                            // If the row does not have any data, break out of the loop
                            if (!hasData)
                            {
                                break;
                            }
                            var user = new CompanyUsers
                            {
                                FirstName = SD.GetCellValue(spreadsheetDocument, cells[0]),
                                LastName = SD.GetCellValue(spreadsheetDocument, cells[1]),
                                Email = SD.GetCellValue(spreadsheetDocument, cells[2]),
                                Department = SD.GetCellValue(spreadsheetDocument, cells[3]),
                                MobileNumber = SD.GetCellValue(spreadsheetDocument, cells[4]),
                                ReadPermission = ConvertToBoolean(SD.GetCellValue(spreadsheetDocument, cells[5])),
                                WritePermission = ConvertToBoolean(SD.GetCellValue(spreadsheetDocument, cells[6])),
                                UploadPermission = ConvertToBoolean(SD.GetCellValue(spreadsheetDocument, cells[7]))
                            };

                            // Check if email already exists
                            bool emailExists = await _context.CompanyUsers.AnyAsync(c => c.Email == user.Email);
                            bool userEmailExists = await _userManager.FindByEmailAsync(user.Email) != null;

                            if (emailExists || userEmailExists)
                            {
                                errorMessages.Add($"Email '{user.Email}' already exists.");
                                // Continue to the next iteration without returning BadRequest immediately
                                continue;
                            }

                            // Add a success message for each successful user addition
                            successMessages.Add($"User '{user.FirstName} {user.LastName}' added successfully.");

                            users.Add(user);
                        }

                        if (users.Count > 0)
                        {
                            // Assign the UserId and AspId for all users
                            foreach (var user in users)
                            {
                                var applicationUser = new ApplicationUser()
                                {
                                    FullName = user.FirstName + " " + user.LastName,
                                    EmailAddress = user.Email,
                                    ConfirmEmailAddress = user.Email,
                                    UserName = user.Email,
                                    PhoneNumber = user.MobileNumber
                                };
                                var password = SD.CreatePassword();
                                var result = await _userManager.CreateAsync(applicationUser, password);

                                if (result.Succeeded)
                                {
                                    await _userManager.AddToRoleAsync(applicationUser, SD.Role_User);

                                    // Associate the user with the company admin user
                                    user.UserId = aspUser.Id;
                                    user.AspId = applicationUser.Id;
                                    user.ApplicationUser = applicationUser;

                                    // Add the user to the CompanyUsers table
                                    _context.CompanyUsers.Add(user);
                                }
                                else
                                {
                                    var errorMessage = result.Errors.Select(error => error.Description);
                                    errorMessages.AddRange(errorMessage);
                                }
                            }
                            // Save all the changes to the database
                            await _context.SaveChangesAsync();

                            if (successMessages.Any())
                            {
                                return Json(new { success = true, successMessage = successMessages, errorMessage = errorMessages });
                            }
                            else
                            {
                                return Json(new { success = false, errorMessage = errorMessages });
                            }
                        }
                    }
                }
                return Json(new { success = false, errorMessage = "Something went wrong." });
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                return Json(new { success = false, errors = new[] { errorMessage } });
            }
        }

        private bool ConvertToBoolean(string value)
        {
            if (bool.TryParse(value, out bool result))
            {
                return result;
            }
            else
            {
                // Handle other variations of true/false values
                return value.Equals("yes", StringComparison.OrdinalIgnoreCase) ||
                       value.Equals("true", StringComparison.OrdinalIgnoreCase);
            }
        }

        [HttpGet]
        public IActionResult GetUser(int id)
        {
            var user = _context.CompanyUsers.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                var userDto = new
                {
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    department = user.Department,
                    mobileNumber = user.MobileNumber,
                    readPermission = user.ReadPermission,
                    writePermission = user.WritePermission,
                    uploadPermission = user.UploadPermission
                };

                return Json(userDto);
            }

            return Json(null);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CompanyUsers user)
        {

            bool emailExists = await _context.CompanyUsers.AnyAsync(c => c.Email == user.Email);
            bool userEmailExists = await _userManager.FindByEmailAsync(user.Email) != null;

            if (emailExists || userEmailExists)
            {
                ModelState.AddModelError("ReciveEmail", "Email already exist");
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return Json(new { success = false, errors = errorMessages });
            }

            if (ModelState.IsValid)
            {

                var applicationUser = new ApplicationUser()
                {
                    FullName = user.FirstName + " " + user.LastName,
                    EmailAddress = user.Email,
                    ConfirmEmailAddress = user.Email,
                    UserName = user.Email,
                    PhoneNumber = user.MobileNumber
                };

                var password = SD.CreatePassword();

                try
                {
                    // Create Company Admin Credential details
                    var result = await _userManager.CreateAsync(applicationUser, password);

                    if (result.Succeeded)
                    {
                        try
                        {
                            await _userManager.AddToRoleAsync(applicationUser, SD.Role_User);

                            var claimIdentity = (ClaimsIdentity)User.Identity;
                            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                            var Userdata = _context.ApplicationUsers.Find(claim.Value);

                            CompanyUsers users = new CompanyUsers()
                            {
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                Department = user.Department,
                                MobileNumber = user.MobileNumber,
                                UserId = Userdata.Id,
                                AspId = applicationUser.Id,
                                ApplicationUser = applicationUser,
                                ReadPermission = user.ReadPermission,
                                WritePermission = user.WritePermission,
                                UploadPermission = user.UploadPermission
                            };

                            _context.CompanyUsers.Add(users);
                            _context.SaveChanges();

                            // Send Email To Created
                            await _emailSender.SendEmailAsync(user.Email, "Your User Credential details", "Your User Name: "
                                + user.Email + "\nYour Password: " + password);

                            return Json(new { success = true, message = " Company User Created Successfully...." });
                            //return Json(new { success = true, data = RedirectToAction("Index") });
                        }
                        catch (Exception ex)
                        {

                            // Log or examine the exception details
                            Console.WriteLine(ex.ToString());

                            throw; // Rethrow the exception or handle it as appropriate
                        }
                    }
                    else
                    {
                        var errorMessages = result.Errors.Select(error => error.Description);
                        return Json(new { success = false, errors = errorMessages });
                    }
                }
                catch (Exception ex)
                {

                    string errorMessage = ex.Message;
                    return Json(new { success = false, errors = new[] { errorMessage } });
                }


            }
            else
            {
                // Create a new list to hold the modified error messages
                var modifiedErrors = new List<string>();

                // Add validation errors to ModelState
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    if (!string.IsNullOrEmpty(error.ErrorMessage))
                    {
                        // Replace the characters in the error message
                        var modifiedErrorMessage = error.ErrorMessage.Replace("'", "\\'");
                        modifiedErrors.Add(modifiedErrorMessage);
                    }
                }

                // Return the modified error messages
                return BadRequest(new { success = false, errors = modifiedErrors });
            }
        }

        [HttpPost]
        public IActionResult UpdateUser(CompanyUsers users)
        {
            if (users == null)
            {
                return BadRequest(new { success = false, error = "Invalid user data" });
            }

            if (!ModelState.IsValid)
            {
                // Create a new list to hold the modified error messages
                var modifiedErrors = new List<string>();

                // Add validation errors to ModelState
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    if (!string.IsNullOrEmpty(error.ErrorMessage))
                    {
                        // Replace the characters in the error message
                        var modifiedErrorMessage = error.ErrorMessage.Replace("'", "\\'");
                        modifiedErrors.Add(modifiedErrorMessage);
                    }
                }

                // Return the modified error messages
                return BadRequest(new { success = false, errors = modifiedErrors });

            }

            var aspUser = _context.CompanyUsers.Find(users.Id);

            if (aspUser != null)
            {
                // Capture the user's first name before updating
                string updatedUserName = aspUser.FirstName; // Replace with the actual property name

                var aspId = _context.ApplicationUsers.Where(c => c.Id == aspUser.AspId).FirstOrDefault();
                if (aspId != null)
                {
                    aspId.FullName = users.FirstName + " " + users.LastName;
                    aspId.PhoneNumber = users.MobileNumber;
                }

                aspUser.FirstName = users.FirstName;
                aspUser.LastName = users.LastName;
                aspUser.Email = users.Email;
                aspUser.Department = users.Department;
                aspUser.MobileNumber = users.MobileNumber;
                aspUser.ReadPermission = users.ReadPermission;
                aspUser.WritePermission = users.WritePermission;
                aspUser.UploadPermission = users.UploadPermission;

                _context.CompanyUsers.Update(aspUser);
                _context.SaveChanges();

                return Json(new { success = true, message = $"User '{updatedUserName}' updated successfully." });
            }

            return Json(new { success = false, message = "User not found." });
        }




        [HttpDelete]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var aspUser = _context.CompanyUsers.Include(i => i.ApplicationUser).Where(i => i.Id == id).FirstOrDefault();

                if (aspUser != null)
                {
                    // Capture the user's first name
                    string deletedUserName = aspUser.FirstName; // Replace with the actual property name

                    if (aspUser.ApplicationUser != null)
                    {
                        _context.ApplicationUsers.Remove(aspUser.ApplicationUser);
                    }
                    _context.CompanyUsers.Remove(aspUser);

                    _context.SaveChanges();

                    return Json(new { success = true, message = $"User '{deletedUserName}' deleted successfully." });
                }

                return Json(new { success = false, message = "User not found." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
                // throw; // Note: You may choose to throw the exception or not, depending on your application's error handling strategy.
            }
        }



        //[HttpDelete]
        //public IActionResult DeleteUser(int id)
        //{
        //    try
        //    {
        //        var aspUser = _context.CompanyUsers.Include(i => i.ApplicationUser).Where(i => i.Id == id).FirstOrDefault();

        //        if (aspUser != null)
        //        {
        //            if (aspUser.ApplicationUser != null)
        //            {
        //                _context.ApplicationUsers.Remove(aspUser.ApplicationUser);
        //            }
        //            _context.CompanyUsers.Remove(aspUser);

        //        }
        //        _context.SaveChanges();

        //        return Json(new { success = true, message = "Company User Deleted Successfully.." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //        throw;
        //    }            
        //}

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            bool isLocked = false;
            var ids = Convert.ToInt32(id);
            var userID = _context.CompanyUsers.Find(ids);
            var userInDb = _context.ApplicationUsers.FirstOrDefault(u => u.Id == userID.AspId);
            if (userInDb == null)
                return Json(new { success = false, message = "Something went wrong while Lock and Unlock User..." });
            if (userInDb != null && userInDb.LockoutEnd > DateTime.Now)
            {
                userInDb.LockoutEnd = DateTime.Now;
                isLocked = false;
            }
            else
            {
                userInDb.LockoutEnd = DateTime.Now.AddYears(50);
                isLocked = true;
            }
            _context.SaveChanges();
            return Json(new
            {
                success = true,
                message = isLocked == true ? "User Successfully Locked" :
                "User Successfully Unlocked"
            });
        }

        //Download pdf file
        public IActionResult ExportPdf()
        {
            try
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    var data = _context.CompanyUsers.Where(c => c.UserId == claim.Value).ToList();
                    if (data != null && data.Count > 0)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            using (PdfDocument document = new PdfDocument())
                            {
                                PdfPage page = document.AddPage();
                                page.Size = PageSize.A4;
                                page.Orientation = PageOrientation.Portrait;

                                XGraphics gfx = XGraphics.FromPdfPage(page);

                                // Define a font size with bold or not bold acording to table header and table data
                                XFont font = new XFont("Verdana", 10, XFontStyle.Regular);
                                XFont boldFont = new XFont("Arial", 14, XFontStyle.Bold);
                                XFont headerFont = new XFont("Arial", 10, XFontStyle.Bold);

                                // Define the starting position of the table
                                double x = 50;
                                double y = 50;

                                gfx.DrawString("Company Users", boldFont, XBrushes.Black, new XPoint(x, y));

                                y += 30; // Move to the next row for headers
                                double colX = x;
                                // Add content to the PDF document
                                gfx.DrawString("First Name", headerFont, XBrushes.Black, new XPoint(colX, y));
                                colX += 60;
                                gfx.DrawString("Last Name", headerFont, XBrushes.Black, new XPoint(colX, y));
                                colX += 60;
                                gfx.DrawString("Email", headerFont, XBrushes.Black, new XPoint(colX, y));
                                colX += 140;
                                gfx.DrawString("Department", headerFont, XBrushes.Black, new XPoint(colX, y));
                                colX += 150;
                                gfx.DrawString("MobileNumber", headerFont, XBrushes.Black, new XPoint(colX, y));
                                colX += 80;
                                gfx.DrawString("Permissions of R/W/U", headerFont, XBrushes.Black, new XPoint(colX, y));

                                // Add table rows dynamically
                                foreach (var companyUsers in data)
                                {
                                    y += 20; // Move to the next row
                                    colX = x;

                                    gfx.DrawString(companyUsers.FirstName ?? "N/A", font, XBrushes.Black, new XPoint(colX, y));
                                    colX += 60;
                                    gfx.DrawString(companyUsers.LastName ?? "N/A", font, XBrushes.Black, new XPoint(colX, y));
                                    colX += 60;
                                    gfx.DrawString(companyUsers.Email ?? "N/A", font, XBrushes.Black, new XPoint(colX, y));
                                    colX += 140;
                                    gfx.DrawString(companyUsers.Department ?? "N/A", font, XBrushes.Black, new XPoint(colX, y));
                                    colX += 150;
                                    gfx.DrawString(companyUsers.MobileNumber ?? "N/A", font, XBrushes.Black, new XPoint(colX, y));
                                    colX += 80;
                                    gfx.DrawString(companyUsers.ReadPermission.ToString() + ", " + companyUsers.WritePermission.ToString() +
                                        ", " + companyUsers.UploadPermission.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                }
                                // Save the document to the stream before closing
                                document.Save(stream);

                                // Close the document
                                document.Close();

                                // Set the position of the stream back to 0
                                stream.Seek(0, SeekOrigin.Begin);
                                Console.WriteLine($"PDF file size: {stream.Length} bytes");

                                string filename = $"Users_{DateTime.Now.ToString("ddMMyyyy")}.pdf";
                                return File(stream.ToArray(), "application/pdf", filename);
                            }
                        }
                    }
                }
                TempData["Error"] = "Data Not Found!";
            }
            catch (Exception ex)
            {
                // Handle the exception
            }

            return RedirectToAction("Index");
        }

        //Download Excel file
        public IActionResult ExportExcel()
        {
            try
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    var data = _context.CompanyUsers.Where(c => c.UserId == claim.Value).ToList();
                    if (data != null && data.Count > 0)
                    {
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            var ws = wb.Worksheets.Add("CompanyUsers");

                            // Add table headers
                            ws.Cell(1, 1).Value = "FirstName";
                            ws.Cell(1, 2).Value = "LastName";
                            ws.Cell(1, 3).Value = "Email";
                            ws.Cell(1, 4).Value = "Department";
                            ws.Cell(1, 5).Value = "MobileNumber";
                            ws.Cell(1, 6).Value = "ReadPermission";
                            ws.Cell(1, 7).Value = "WritePermission";
                            ws.Cell(1, 8).Value = "UplodePermission";

                            // Add data to the table
                            for (int i = 0; i < data.Count; i++)
                            {
                                ws.Cell(i + 2, 1).Value = data[i].FirstName;
                                ws.Cell(i + 2, 2).Value = data[i].LastName;
                                ws.Cell(i + 2, 3).Value = data[i].Email;
                                ws.Cell(i + 2, 4).Value = data[i].Department;
                                ws.Cell(i + 2, 5).Value = data[i].MobileNumber;
                                ws.Cell(i + 2, 6).Value = data[i].ReadPermission;
                                ws.Cell(i + 2, 7).Value = data[i].WritePermission;
                                ws.Cell(i + 2, 8).Value = data[i].UploadPermission;
                            }

                            using (MemoryStream stream = new MemoryStream())
                            {
                                wb.SaveAs(stream);
                                string filename = $"CompanyUser_{DateTime.Now.ToString("ddMMyyyy")}.xlsx";
                                stream.Position = 0;
                                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                            }
                        }
                    }
                    TempData["Error"] = "Data Not Found!";
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log, or display an error message
            }
            return RedirectToAction("Index");
        }

        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var users = _context.CompanyUsers.Where(c => c.UserId == claim.Value).Include(c => c.ApplicationUser).ToList();

                // Select the specific fields to display
                var userData = users.Select(u => new
                {
                    fullName = u.FirstName + " " + u.LastName,
                    email = u.Email,
                    department = u.Department,
                    mobileNumber = u.MobileNumber,
                    readPermission = u.ReadPermission,
                    writePermission = u.WritePermission,
                    uploadPermission = u.UploadPermission,
                    id = u.Id,
                    applicationUser = new
                    {
                        lockoutEnd = u.ApplicationUser.LockoutEnd
                    }
                }).ToList();

                return Json(new { data = userData });
            }

            return Json(new { data = new List<object>() });
        }
        #endregion

    }
}