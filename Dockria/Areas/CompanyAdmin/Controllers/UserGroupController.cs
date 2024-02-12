using ClosedXML.Excel;
using Dockria.Data;
using DocumentFormat.OpenXml.Presentation;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;
using System.Data;
using System.Drawing;
using System.Security.Claims;
using System.Text;

namespace WebApp.Areas.CompanyAdmin.Controllers
{
    [Area("CompanyAdmin")]
    public class UserGroupController : Controller
    {
        private ApplicationDbContext _context;

        public UserGroupController(ApplicationDbContext context)
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
        public JsonResult UserName()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var users = _context.CompanyUsers.Where(c => c.UserId == claim.Value).Include(c => c.ApplicationUser).ToList();

                var user = users.Select(x => new { Value = x.UserId, Text = x.FirstName + " " + x.LastName }).ToList();

                var userData = new { userlist = user };

                return Json(userData);
            }
            return Json(new { success = false, message = "Data Not Found..." });
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] UserGroup userGroup)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                if (ModelState.IsValid)
                {
                    var users = await _context.CompanyAdmin.FirstOrDefaultAsync(u => u.AspId == claim.Value);
                    if (users != null)
                    {
                        try
                        {
                            // Check if a UserGroup with the same name already exists
                            bool userGroupExists = await _context.UserGroups.Where(c => c.CompanyAdminId == users.Id)
                                .AnyAsync(u => u.UserGroupName == userGroup.UserGroupName);

                            if (userGroupExists)
                            {
                                return Json(new { success = false, message = "User group name already exists" });
                            }

                            // Create a new instance of the UserGroup model
                            userGroup.CompanyAdminId = users.Id;
                            userGroup.CompanyAdminUser = users;

                            _context.UserGroups.Add(userGroup);
                            await _context.SaveChangesAsync();

                            return Json(new { success = true, message = " User Group Created Successfully...." });

                        }
                        catch (Exception ex)
                        {

                            // Log or examine the exception details
                            Console.WriteLine(ex.ToString());
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
            }
            return BadRequest(new { success = false, errors = "Data not found..." });
        }

        [HttpGet]
        public IActionResult GetGroupUser(int id)
        {
            var usergroup = _context.UserGroups.SingleOrDefault(x => x.UserGroupId == id);
            if (usergroup != null)
            {
                var userDto = new
                {
                    groupName = usergroup.UserGroupName,
                    username = usergroup.UserName
                };

                return Json(userDto);
            }

            return Json(null);
        }

        [HttpPost]
        public async Task<IActionResult> EditGroupUser([FromBody] UserGroup model)
        {
            if (ModelState.IsValid)
            {
                // Fetch the existing user group from the database
                var existingGroup = await _context.UserGroups.FindAsync(model.UserGroupId);

                if (existingGroup != null)
                {
                    // Update the properties of the existing user group
                    existingGroup.UserGroupName = model.UserGroupName;
                    existingGroup.UserName = model.UserName;

                    // Save the changes to the database
                    _context.UserGroups.Update(existingGroup);
                    await _context.SaveChangesAsync();

                    // Redirect to a success page or return a success message
                    return Json(new { success = true, message = "  User Group Updated Successfully...." });
                }
            }

            // If there is an error or invalid data, return to the edit view with the model
            return View("EditGroupUser", model);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            try
            {
                var userGroup = _context.UserGroups.Find(id);
                if (userGroup != null)
                {
                    var endUserPermission = await _context.EndUserManagements.Where(x => x.GroupId == userGroup.UserGroupId).FirstOrDefaultAsync();
                    if (endUserPermission != null) _context.EndUserManagements.Remove(endUserPermission);

                    var radPermission = await _context.RADManagements.Where(x => x.GroupId == userGroup.UserGroupId).FirstOrDefaultAsync();
                    if (radPermission != null) _context.RADManagements.Remove(radPermission);

                    var userDocPermission = await _context.DocumentManagements.Where(x => x.GroupId == userGroup.UserGroupId).FirstOrDefaultAsync();
                    if (userDocPermission != null) _context.DocumentManagements.Remove(userDocPermission);
                    await _context.SaveChangesAsync();

                    _context.UserGroups.Remove(userGroup);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "User Group Deleted Successfully.." });
                }

                return Json(new { success = false, message = "User Group Not Found..." });
            }
            catch (Exception ex)
            {
                // Handle the exception here
                return Json(new { success = false, message = "An error occurred while deleting the data." });
            }

        }

        private bool IsUserGroupSelected(int userId, string groupName)
        {
            var userGroup = _context.UserGroups.FirstOrDefault(g => g.UserName == groupName);
            if (userGroup != null)
            {
                return userGroup.UsersList?.Contains(userId.ToString()) ?? false;
            }
            return false;
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
                    var data = _context.UserGroups.Where(i => i.CompanyAdminUser.AspId == claim.Value).ToList();
                    if (data != null && data.Count > 0)
                    {
                        // Select only the "GroupName" and "UserName" columns from the data
                        var filteredData = data.Select(group => new
                        {
                            GroupName = group.UserGroupName,
                            UserName = group.UserName
                        }).ToList();

                        if (filteredData.Count > 0)
                        {
                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                var ws = wb.Worksheets.Add("UserGroups");

                                // Add table headers
                                ws.Cell(1, 1).Value = "GroupName";
                                ws.Cell(1, 2).Value = "UserName";

                                // Add data to the table
                                for (int i = 0; i < filteredData.Count; i++)
                                {
                                    ws.Cell(i + 2, 1).Value = filteredData[i].GroupName;
                                    ws.Cell(i + 2, 2).Value = filteredData[i].UserName;
                                }

                                using (MemoryStream stream = new MemoryStream())
                                {
                                    wb.SaveAs(stream);
                                    string filename = $"UserGroup_{DateTime.Now.ToString("ddMMyyyy")}.xlsx";
                                    stream.Position = 0;
                                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                                }
                            }
                        }
                        else
                        {
                            TempData["Error"] = "Data Not Found!";
                        }
                    }
                    else
                    {
                        TempData["Error"] = "Data Not Found!";
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log, or display an error message
            }

            return RedirectToAction("Index");
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
                    var data = _context.UserGroups.Where(i => i.CompanyAdminUser.AspId == claim.Value).ToList();
                    if (data != null && data.Count > 0)
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            using (PdfDocument document = new PdfDocument())
                            {
                                PdfPage page = document.AddPage();
                                XGraphics gfx = XGraphics.FromPdfPage(page);

                                // Add content to the PDF document
                                XFont font = new XFont("Verdana", 10, XFontStyle.Regular);
                                XFont boldFont = new XFont("Arial", 14, XFontStyle.Bold);

                                // Define the starting position of the table
                                double x = 50;
                                double y = 50;

                                gfx.DrawString("User Groups", boldFont, XBrushes.Black, new XPoint(x + 300, y));
                                y += 20;

                                // Draw table headers
                                double groupNameWidth = 150;
                                double userNameWidth = 420;
                                double cellHeight = 50; // Default cell height

                                // Draw table headers
                                DrawCell(gfx, "Group Name", font, x, y, groupNameWidth, cellHeight);
                                DrawCell(gfx, "User Name", font, x + groupNameWidth, y, userNameWidth, cellHeight);
                                y += cellHeight;

                                // Add table rows dynamically
                                foreach (var userGroup in data)
                                {
                                    // Wrap the text within the cell based on words
                                    string userName = userGroup.UserName;
                                    List<string> lines = new List<string>();
                                    var format = new XStringFormat();

                                    XRect groupNameRect = new XRect(x, y, groupNameWidth, cellHeight);
                                    XRect userNameRect = new XRect(x + groupNameWidth, y, userNameWidth, cellHeight);

                                    // Split the user name into lines to wrap the content
                                    string[] words = userName.Split(' ');
                                    StringBuilder line = new StringBuilder();

                                    foreach (var word in words)
                                    {
                                        double lineWidth = gfx.MeasureString(line + word, font).Width;

                                        if (lineWidth < userNameRect.Width)
                                        {
                                            line.Append(word + " ");
                                        }
                                        else
                                        {
                                            lines.Add(line.ToString());
                                            line.Clear();
                                            line.Append(word + " ");
                                        }
                                    }
                                    lines.Add(line.ToString());

                                    // Calculate the height required for the wrapped text
                                    double wrappedTextHeight = lines.Count * font.Height;

                                    // Calculate the maximum height needed for the current row
                                    double maxRowHeight = Math.Max(wrappedTextHeight, cellHeight);

                                    // Draw the group name cell with the maximum row height
                                    DrawCell(gfx, userGroup.UserGroupName, font, x, y, groupNameWidth, maxRowHeight);

                                    // Draw the wrapped text in the user name cell, adjusting the height to match the group name cell
                                    for (int i = 0; i < lines.Count; i++)
                                    {
                                        double lineY = y + i * (maxRowHeight / lines.Count);
                                        DrawCell(gfx, lines[i], font, x + groupNameWidth, lineY, userNameWidth, maxRowHeight / lines.Count);
                                    }
                                    y += maxRowHeight; // Move to the next row
                                }

                                document.Save(stream);

                                // Set the position of the stream back to 0
                                stream.Position = 0;

                                string filename = $"UserGroup_{DateTime.Now.ToString("ddMMyyyy")}.pdf";
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

        // Helper method to draw a cell with a border
        private void DrawCell(XGraphics gfx, string content, XFont font, double x, double y, double width, double height)
        {
            XRect rect = new XRect(x, y, width, height);

            // Draw cell border
            gfx.DrawRectangle(XPens.Black, rect);

            // Center the content within the cell
            var format = new XStringFormat
            {
                Alignment = XStringAlignment.Center,
                LineAlignment = XLineAlignment.Center
            };

            gfx.DrawString(content, font, XBrushes.Black, rect, format);
        }

        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var users = _context.CompanyAdmin.Where(c => c.AspId == claim.Value).ToList();
                var userlist = new List<object>();

                foreach (var user in users)
                {
                    var userGroupList = _context.UserGroups.Where(x => x.CompanyAdminId == user.Id).ToList();

                    var userGroups = userGroupList.Select(u => new
                    {
                        GroupName = u.UserGroupName,
                        Id = u.UserGroupId,
                    }).ToList();

                    userlist.AddRange(userGroups);
                }

                return Json(new { data = userlist });
            }

            return Json(new { data = new List<object>() });
        }

        [HttpGet]
        public IActionResult GetAllUser()
        {
            var userList = _context.CompanyUsers.ToList();
            var groupList = _context.UserGroups.ToList();

            var userData = userList.Select(u => new
            {
                GroupData = groupList.Select(g => new
                {
                    IsSelected = IsUserGroupSelected(u.Id, g.UserGroupName),
                    GroupName = g.UserName
                }).ToList(),

                userName = u.FirstName + " " + u.LastName,
                id = u.Id
            }).ToList();

            return Json(new { data = userData });
        }
        #endregion
    }
}