using ClosedXML.Excel;
using Dockria.Data;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Model;
using Domain.Model.ViewModel;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Crypto.Tls;
using PdfSharpCore.Drawing;
using System.IO.Compression;
using System.IO.Packaging;
using System.Security.Claims;
using PdfDocument = PdfSharpCore.Pdf.PdfDocument;
using PdfPage = PdfSharpCore.Pdf.PdfPage;

namespace WebApp.Areas.CompanyAdmin.Controllers
{
    [Area("CompanyAdmin")]
    public class GroupPermissionController : Controller
    {
        private ApplicationDbContext _context;

        public GroupPermissionController(ApplicationDbContext context)
        {
            _context = context;
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

        private string GenerateRandomCode()
        {
            // Implement your logic to generate a unique random code
            // For example, you can use Guid.NewGuid() to generate a unique identifier
            return Guid.NewGuid().ToString();
        }
        [HttpGet]
        public IActionResult Details(string id)
        {
            // Use the id parameter as needed in your CompanyAdmin action
            ViewBag.RandomString = id;

            // If id is not provided or is empty, generate a new random code
            if (string.IsNullOrEmpty(id))
            {
                string randomCode = GenerateRandomCode();
                ViewBag.RandomString = randomCode;
                return RedirectToAction("Details", new { id = randomCode });
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
                try
                {
                    var users = _context.CompanyAdmin.FirstOrDefault(c => c.AspId == claim.Value);
                    var userList = new List<object>();

                    var userGroupList = _context.UserGroups.Where(x => x.CompanyAdminId == users.Id).ToList();

                    // Create a list to store items to be removed
                    var itemsToRemove = new List<UserGroup>();

                    foreach (var item in userGroupList)
                    {

                        // Check if the UserGroupId exists in any of the three tables
                        bool existsInDocumentManagements = _context.DocumentManagements.Any(dm => dm.GroupId == item.UserGroupId);
                        bool existsInRadManagement = _context.RADManagements.Any(rm => rm.GroupId == item.UserGroupId);
                        bool existsInStructureManagement = _context.EndUserManagements.Any(sm => sm.GroupId == item.UserGroupId);

                        // If it exists in any of the tables, add the item to itemsToRemove list
                        if (existsInDocumentManagements || existsInRadManagement || existsInStructureManagement)
                        {
                            itemsToRemove.Add(item);
                        }
                    }

                    // Remove the items from userGroupList
                    foreach (var itemToRemove in itemsToRemove)
                    {
                        userGroupList.Remove(itemToRemove);
                    }

                    var userGroups = userGroupList.Select(u => new
                    {
                        Value = u.UserGroupId,
                        Text = u.UserGroupName,
                    }).ToList();

                    userList.AddRange(userGroups);

                    var userData = new { userlist = userList };
                    return Json(userData);
                }
                catch (Exception ex)
                {

                    throw;
                }

            }

            return Json(new { success = "false", data = new List<object>() });
        }

        [HttpPost]
        public IActionResult SavePermission([FromBody] UserGroupPermissionVM userGroupPermissionVM)
        {
            if (ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    var users = _context.CompanyAdmin.FirstOrDefault(c => c.AspId == claim.Value);

                    var userGroupsList = _context.UserGroups.Where(i => i.CompanyAdminId == users.Id).ToList();

                    var userGroups = userGroupsList.Where(c => c.UserGroupName == userGroupPermissionVM.UserGroup.UserGroupName).ToList();

                    if (userGroups != null)
                    {
                        var existexistingGroupPermissions = userGroups.Any(u =>
                        _context.DocumentManagements.Any(d => d.GroupId == u.UserGroupId) ||
                        _context.EndUserManagements.Any(e => e.GroupId == u.UserGroupId) ||
                        _context.RADManagements.Any(r => r.GroupId == u.UserGroupId));


                        if (existexistingGroupPermissions) return Json(new { status = false, message = "This Group Allready have Exists permissions" });

                        var group = userGroups.FirstOrDefault();

                        // Add End User Management Data into database..
                        userGroupPermissionVM.EndUserManagement.GroupId = group.UserGroupId;
                        userGroupPermissionVM.EndUserManagement.UserGroup = group;
                        _context.EndUserManagements.Add(userGroupPermissionVM.EndUserManagement);


                        // Add RAD Management Data into database...
                        userGroupPermissionVM.RADManagement.GroupId = group.UserGroupId;
                        userGroupPermissionVM.RADManagement.UserGroup = group;
                        _context.RADManagements.Add(userGroupPermissionVM.RADManagement);

                        // Add DocumentManagement Data into database...
                        userGroupPermissionVM.DocumentManagement.GroupId = group.UserGroupId;
                        userGroupPermissionVM.DocumentManagement.UserGroup = group;
                        _context.DocumentManagements.Add(userGroupPermissionVM.DocumentManagement);

                        _context.SaveChanges();


                        return Json(new { success = true, message = "GroupPermission Saved Successfully.." });
                    }
                    else
                    {
                        return Json(new { status = false, message = "User Group not found" });
                    }
                }
                else
                {
                    return Json(new { status = false, message = "User Group not found" });
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return Json(new
                {
                    status = false,
                    message = "Model validation error",
                    errors = errors
                });
            }
        }

        [HttpGet]
        public IActionResult GetRadDetails(int id)
        {
            var groupData = _context.UserGroups.Find(id);
            if (groupData != null)
            {
                var radData = _context.RADManagements.FirstOrDefault(u => u.GroupId == groupData.UserGroupId);
                if (radData != null)
                {
                    return Json(new { status = true, data = radData });
                }
            }
            return Json(new { status = false, message = "Data Not Found...." });
        }

        [HttpPost]
        public IActionResult UpdateRadDetails(RADManagement radData)
        {
            var dms = _context.RADManagements.Find(radData.Id);
            if (dms != null)
            {
                try
                {
                    dms.RadView = radData.RadView;
                    dms.RadEdit = radData.RadEdit;
                    dms.RadFormFill = radData.RadFormFill;

                    _context.RADManagements.Update(dms);
                    _context.SaveChanges();

                    return Json(new { success = true, message = "RAD Updated Successfully.." });
                }
                catch (Exception ex)
                {

                    // Log or examine the exception details
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            }
            return Json(new { success = false, message = "Something wrong while update data..." });
        }

        [HttpGet]
        public IActionResult GetDMSDetails(int id)
        {
            var groupData = _context.UserGroups.Find(id);
            if (groupData != null)
            {
                var dmsData = _context.DocumentManagements.FirstOrDefault(u => u.GroupId == groupData.UserGroupId);
                if (dmsData != null)
                {
                    return Json(new { status = true, data = dmsData });
                }
            }
            return Json(new { status = false, message = "Data Not Found...." });
        }

        [HttpPost]
        public IActionResult UpdateDms(DocumentManagement dmsData)
        {
            var dms = _context.DocumentManagements.Find(dmsData.Id);
            if (dms != null)
            {
                try
                {
                    dms.ViewDoc = dmsData.ViewDoc;
                    dms.DocSinAdd = dmsData.DocSinAdd;
                    dms.DocMulAdd = dmsData.DocMulAdd;
                    dms.DocCopy = dmsData.DocCopy;
                    dms.DocMove = dmsData.DocMove;
                    dms.DocDelete = dmsData.DocDelete;
                    dms.DocRename = dmsData.DocRename;
                    dms.DocPrivate = dmsData.DocPrivate;
                    dms.DocDown = dmsData.DocDown;
                    dms.DocPrint = dmsData.DocPrint;
                    dms.ViewMatadata = dmsData.ViewMatadata;
                    dms.EditMatadata = dmsData.EditMatadata;
                    dms.ShareDocInt = dmsData.ShareDocInt;
                    dms.ShareDocExt = dmsData.ShareDocExt;
                    dms.ShareSigExt = dmsData.ShareSigExt;
                    dms.AuditLogDoc = dmsData.AuditLogDoc;
                    dms.DocVerView = dmsData.DocVerView;
                    dms.DocRollBack = dmsData.DocRollBack;
                    dms.DownCsvRpt = dmsData.DownCsvRpt;
                    dms.AuditLogUser = dmsData.AuditLogUser;
                    dms.AsgnDocUser = dmsData.AsgnDocUser;
                    dms.MaxDocUpSize = dmsData.MaxDocUpSize;
                    dms.MaxDocUpNum = dmsData.MaxDocUpNum;

                    _context.DocumentManagements.Update(dms);
                    _context.SaveChanges();
                    return Json(new { success = true, message = "DMS Updated Successfully.." });
                }
                catch (Exception ex)
                {
                    // Log or examine the exception details
                    Console.WriteLine(ex.ToString());
                    throw;
                }

            }
            return Json(new { success = false, message = "Something wrong while update data..." });
        }

        [HttpGet]
        public IActionResult GetEndUserDetails(int id)
        {
            var groupData = _context.UserGroups.Find(id);
            if (groupData != null)
            {
                var endUserData = _context.EndUserManagements.FirstOrDefault(u => u.GroupId == groupData.UserGroupId);
                if (endUserData != null)
                    return Json(new { status = true, data = endUserData });
            }
            return Json(new { status = false, message = "Data Not Found...." });
        }

        [HttpPost]
        public IActionResult UpdateEndUserDetails(EndUserManagement endUserData)
        {
            var dms = _context.EndUserManagements.Find(endUserData.Id);
            if (dms != null)
            {
                try
                {
                    dms.EditEmail = endUserData.EditEmail;
                    dms.EditSign = endUserData.EditSign;
                    dms.EditPassword = endUserData.EditPassword;

                    _context.EndUserManagements.Update(dms);
                    _context.SaveChanges();

                    return Json(new { success = true, message = "EndUser Updated Successfully.." });
                }
                catch (Exception ex)
                {

                    // Log or examine the exception details
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            }
            return Json(new { status = false, message = "Data Not Found...." });
        }

        // download Pdf file for all group details
        public IActionResult ExportPdf()
        {
            try
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    var companyAdmin = _context.CompanyAdmin.FirstOrDefault(i => i.AspId == claim.Value);
                    if (companyAdmin != null)
                    {
                        // Get the UserGroup data from the database
                        var userGroups = _context.UserGroups?.Where(i => i.CompanyAdminId == companyAdmin.Id).ToList();
                        if (userGroups != null && userGroups.Count > 0)
                        {
                            using (MemoryStream stream = new MemoryStream())
                            {
                                using (PdfDocument document = new PdfDocument())
                                {
                                    PdfPage page = document.AddPage();
                                    XGraphics gfx = XGraphics.FromPdfPage(page);
                                    XFont font = new XFont("Verdana", 10, XFontStyle.Regular);

                                    // Define a bold font with increased size
                                    XFont boldFont = new XFont("Arial", 12, XFontStyle.Bold);
                                    XFont headerFont = new XFont("Arial", 10, XFontStyle.Bold);

                                    // Define the starting position of the table
                                    double x = 50;
                                    double y = 100;

                                    gfx.DrawString("User Groups", boldFont, XBrushes.Black, new XPoint(x, y));
                                    y += 20;

                                    // Add content to the PDF document
                                    gfx.DrawString("Group Name", headerFont, XBrushes.Black, new XPoint(x, y));
                                    gfx.DrawString("User Name", headerFont, XBrushes.Black, new XPoint(x + 100, y));

                                    // Add table rows dynamically
                                    foreach (var userGroup in userGroups)
                                    {
                                        y += 20; // Move to the next row
                                        gfx.DrawString(userGroup.UserGroupName, font, XBrushes.Black, x, y);
                                        gfx.DrawString(userGroup.UserName, font, XBrushes.Black, x + 100, y);
                                    }

                                    // Add the DocumentManagement data to the PDF document
                                    var documentManagementData = _context.DocumentManagements.Where(i => i.UserGroup.CompanyAdminId == companyAdmin.Id).ToList();
                                    if (documentManagementData != null && documentManagementData.Count > 0)
                                    {
                                        y += 40; // Add some space between permission groups
                                        gfx.DrawString("Document Management Permissions", boldFont, XBrushes.Black, new XPoint(x, y));

                                        // Create a new table for DocumentManagement data
                                        y += 20; // Move to the next row for headers
                                        double colX = x;
                                        int columnsPerPage = 5;

                                        // Define header names in an array for easy iteration
                                        string[] headerNames = new string[]
                                        {
                                         "Group Name", "View Doc", "Add Single Doc",
                                         "Add Multiple Doc", "Copy Doc", "Move Doc", "Delete Doc",
                                          "Rename Doc", "Private Doc", "Download Doc", "Print Doc",
                                         "View Metadata", "Edit Metadata", "Share Doc Internal",
                                         "Share Doc External", "Share Signature External", "Audit log Doc",
                                         "Doc Version View", "Doc RollBack", "Download CSV Report",
                                         "Audit log Users", "Assign Doc Users", "Max Doc Upload Size",
                                         "Max Doc Upload Number"
                                        };

                                        // Calculate the number of pages needed for the table based on the number of columns
                                        int totalPages = (int)Math.Ceiling((double)headerNames.Length / columnsPerPage);

                                        for (int pageIndex = 0; pageIndex < totalPages; pageIndex++)
                                        {
                                            // Create a new page for each batch of columns
                                            page = document.AddPage();
                                            gfx = XGraphics.FromPdfPage(page);
                                            x = 50;
                                            y = 50; // Reset the Y position for the new page
                                            colX = x;

                                            // Calculate the width of each column
                                            double columnWidth = 100;
                                            double rowHeight = 20; // Height of each row, adjust as needed

                                            // Add table headers for the first page only
                                            if (pageIndex == 0)
                                            {
                                                y = 50; // Reset the Y position for the new page

                                                // Draw the header text for the current page's columns
                                                int startColumnIndex = pageIndex * columnsPerPage;
                                                int endColumnIndex = Math.Min(startColumnIndex + columnsPerPage, headerNames.Length);

                                                for (int i = startColumnIndex; i < endColumnIndex; i++)
                                                {
                                                    string[] headerLines = SplitHeaderIntoLines(headerNames[i], headerFont, columnWidth, gfx);

                                                    // Calculate the total height required for the header lines
                                                    double totalHeaderHeight = headerLines.Length * headerFont.GetHeight();

                                                    // Calculate the X position for the header text based on the current column index and column width
                                                    double xPos = x + (i - startColumnIndex) * columnWidth;

                                                    // Calculate the Y position for the header text to center it vertically in the header row
                                                    double yPos = y + (rowHeight - totalHeaderHeight) / 2;

                                                    // Draw each line of the header text for the current column
                                                    for (int lineIndex = 0; lineIndex < headerLines.Length; lineIndex++)
                                                    {
                                                        gfx.DrawString(headerLines[lineIndex], headerFont, XBrushes.Black, new XPoint(xPos, yPos));
                                                        yPos += headerFont.GetHeight();
                                                    }
                                                }

                                                // Update Y position for the data rows
                                                y += rowHeight + 20;
                                            }
                                            else
                                            {
                                                y = 50; // Reset the Y position for the new page

                                                // Draw the header text for the current page's columns
                                                int startColumnIndex = pageIndex * columnsPerPage;
                                                int endColumnIndex = Math.Min(startColumnIndex + columnsPerPage, headerNames.Length);

                                                for (int i = startColumnIndex; i < endColumnIndex; i++)
                                                {
                                                    string[] headerLines = SplitHeaderIntoLines(headerNames[i], headerFont, columnWidth, gfx);

                                                    // Calculate the total height required for the header lines
                                                    double totalHeaderHeight = headerLines.Length * headerFont.GetHeight();

                                                    // Calculate the X position for the header text based on the current column index and column width
                                                    double xPos = x + (i - startColumnIndex) * columnWidth;

                                                    // Calculate the Y position for the header text to center it vertically in the header row
                                                    double yPos = y + (rowHeight - totalHeaderHeight) / 2;

                                                    // Draw each line of the header text for the current column
                                                    for (int lineIndex = 0; lineIndex < headerLines.Length; lineIndex++)
                                                    {
                                                        gfx.DrawString(headerLines[lineIndex], headerFont, XBrushes.Black, new XPoint(xPos, yPos));
                                                        yPos += headerFont.GetHeight();
                                                    }
                                                }

                                                // Update Y position for the data rows
                                                y += rowHeight + 20;
                                            }

                                            // Define startColumnIndex and endColumnIndex outside the if-else block
                                            int startColumnIndexs = pageIndex * columnsPerPage;
                                            int endColumnIndexs = Math.Min(startColumnIndexs + columnsPerPage, headerNames.Length);

                                            // Add table rows dynamically
                                            foreach (var docManagement in documentManagementData)
                                            {
                                                // Reset the column position for each data row
                                                colX = x;

                                                // Draw the data for the current page's columns
                                                for (int i = startColumnIndexs; i < endColumnIndexs; i++)
                                                {
                                                    switch (i)
                                                    {
                                                        case 0:
                                                            gfx.DrawString(docManagement.UserGroup.UserGroupName.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 1:
                                                            gfx.DrawString(docManagement.ViewDoc.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 2:
                                                            gfx.DrawString(docManagement.DocSinAdd.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 3:
                                                            gfx.DrawString(docManagement.DocMulAdd.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 4:
                                                            gfx.DrawString(docManagement.DocCopy.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 5:
                                                            gfx.DrawString(docManagement.DocMove.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 6:
                                                            gfx.DrawString(docManagement.DocDelete.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 7:
                                                            gfx.DrawString(docManagement.DocRename.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 8:
                                                            gfx.DrawString(docManagement.DocPrivate.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 9:
                                                            gfx.DrawString(docManagement.DocDown.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 10:
                                                            gfx.DrawString(docManagement.DocPrint.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 11:
                                                            gfx.DrawString(docManagement.ViewMatadata.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 12:
                                                            gfx.DrawString(docManagement.EditMatadata.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 13:
                                                            gfx.DrawString(docManagement.ShareDocInt.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 14:
                                                            gfx.DrawString(docManagement.ShareDocExt.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 15:
                                                            gfx.DrawString(docManagement.ShareSigExt.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 16:
                                                            gfx.DrawString(docManagement.AuditLogDoc.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 17:
                                                            gfx.DrawString(docManagement.DocVerView.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 18:
                                                            gfx.DrawString(docManagement.DocRollBack.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 19:
                                                            gfx.DrawString(docManagement.DownCsvRpt.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 20:
                                                            gfx.DrawString(docManagement.AuditLogUser.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 21:
                                                            gfx.DrawString(docManagement.AsgnDocUser.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 22:
                                                            gfx.DrawString(docManagement.MaxDocUpSize.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                        case 23:
                                                            gfx.DrawString((docManagement.MaxDocUpNum ?? "0"), font, XBrushes.Black, new XPoint(colX, y));
                                                            break;
                                                    }
                                                    colX += 100;
                                                }

                                                // Move to the next row
                                                y += 20;
                                            }
                                        }
                                    }

                                    // Add the EndUserManagement data to the PDF document
                                    var endUserManagementData = _context.EndUserManagements.Where(i => i.UserGroup.CompanyAdminId == companyAdmin.Id).ToList();
                                    if (endUserManagementData != null && endUserManagementData.Count > 0)
                                    {
                                        y += 40; // Add some space between permission groups
                                        gfx.DrawString("End User Management Permissions", boldFont, XBrushes.Black, new XPoint(x, y));

                                        // Create a new table for EndUserManagement data
                                        y += 20; // Move to the next row for headers
                                        double colX = x;
                                        gfx.DrawString("Group Name", headerFont, XBrushes.Black, new XPoint(colX, y));
                                        colX += 100;
                                        gfx.DrawString("Edit Email", headerFont, XBrushes.Black, new XPoint(colX, y));
                                        colX += 100;
                                        gfx.DrawString("Edit Password", headerFont, XBrushes.Black, new XPoint(colX, y));
                                        colX += 100;
                                        gfx.DrawString("Edit Signature", headerFont, XBrushes.Black, new XPoint(colX, y));

                                        // Add table rows dynamically
                                        foreach (var endUserManagement in endUserManagementData)
                                        {
                                            y += 20; // Move to the next row
                                            colX = x;
                                            gfx.DrawString(endUserManagement.UserGroup.UserGroupName.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                            colX += 100;
                                            gfx.DrawString(endUserManagement.EditEmail.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                            colX += 100;
                                            gfx.DrawString(endUserManagement.EditPassword.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                            colX += 100;
                                            gfx.DrawString(endUserManagement.EditSign.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                            colX += 100;
                                        }

                                        // Update the Y position for the next permission group
                                        y += 20; // Add some space between permission groups
                                    }

                                    // Add the RADManagement data to the PDF document
                                    var radManagementData = _context.RADManagements.Where(i => i.UserGroup.CompanyAdminId == companyAdmin.Id).ToList();
                                    if (radManagementData != null && radManagementData.Count > 0)
                                    {
                                        y += 40; // Add some space between permission groups
                                        gfx.DrawString("RAD Management Permissions", boldFont, XBrushes.Black, new XPoint(x, y));

                                        // Create a new table for RADManagement data                                      
                                        y += 20; // Move to the next row for headers
                                        double colX = x;
                                        gfx.DrawString("Group Name", headerFont, XBrushes.Black, new XPoint(colX, y));
                                        colX += 100;
                                        gfx.DrawString("RAD View", headerFont, XBrushes.Black, new XPoint(colX, y));
                                        colX += 100;
                                        gfx.DrawString("RAD Edit", headerFont, XBrushes.Black, new XPoint(colX, y));
                                        colX += 100;
                                        gfx.DrawString("RAD Form Fill", headerFont, XBrushes.Black, new XPoint(colX, y));
                                        colX += 100;

                                        // Add table rows dynamically
                                        foreach (var radManagement in radManagementData)
                                        {
                                            y += 20; // Move to the next row
                                            colX = x;
                                            gfx.DrawString(radManagement.UserGroup.UserGroupName.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                            colX += 100;
                                            gfx.DrawString(radManagement.RadView.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                            colX += 100;
                                            gfx.DrawString(radManagement.RadEdit.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                            colX += 100;
                                            gfx.DrawString(radManagement.RadFormFill.ToString(), font, XBrushes.Black, new XPoint(colX, y));
                                            colX += 100;
                                        }

                                        document.Save(stream);
                                    }

                                    document.Close();

                                    // Set the position of the stream back to 0
                                    stream.Position = 0;

                                    string filename = $"PermissionData_{DateTime.Now.ToString("ddMMyyyy")}.pdf";
                                    return File(stream.ToArray(), "application/pdf", filename);
                                }
                            }
                        }
                    }
                    TempData["Error"] = "UserGroup Data Not Found!";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // If there is an error or data not found, redirect back to the view
            return RedirectToAction("Details");
        }

        // Helper function to split header text into multiple lines while breaking on complete words
        private string[] SplitHeaderIntoLines(string headerText, XFont font, double maxWidth, XGraphics gfx)
        {
            List<string> lines = new List<string>();
            string[] words = headerText.Split(' ');
            double lineHeight = gfx.MeasureString(headerText, font).Height;

            string currentLine = string.Empty;

            foreach (var word in words)
            {
                string tempLine = currentLine + (string.IsNullOrEmpty(currentLine) ? "" : " ") + word;
                double tempWidth = gfx.MeasureString(tempLine, font).Width;

                if (tempWidth <= maxWidth)
                {
                    currentLine = tempLine;
                }
                else
                {
                    if (!string.IsNullOrEmpty(currentLine))
                    {
                        lines.Add(currentLine);
                    }
                    currentLine = word;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
            }

            return lines.ToArray();
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
                    var data = _context.UserGroups.Where(c => c.CompanyAdminUser.AspId == claim.Value).ToList();
                    if (data != null && data.Count > 0)
                    {
                        using (var package = new ExcelPackage())
                        {
                            // Add "User Groups" worksheet
                            var wsUserGroups = package.Workbook.Worksheets.Add("User Groups");

                            // Add table headers for User Groups
                            wsUserGroups.Cells[1, 1].Value = "Group Name";
                            wsUserGroups.Cells[1, 2].Value = "Users Name";

                            // Add data to the User Groups table
                            for (int i = 0; i < data.Count; i++)
                            {
                                wsUserGroups.Cells[i + 2, 1].Value = data[i].UserGroupName;
                                wsUserGroups.Cells[i + 2, 2].Value = data[i].UserName;
                            }

                            // Add "End User Management Permissions" worksheet
                            var wsEndUserManagement = package.Workbook.Worksheets.Add("End User Management Permissions");


                            // Add table headers for End User Management
                            wsEndUserManagement.Cells[1, 1].Value = "Group Name";
                            wsEndUserManagement.Cells[1, 2].Value = "Edit Email";
                            wsEndUserManagement.Cells[1, 3].Value = "Edit Password";
                            wsEndUserManagement.Cells[1, 4].Value = "Edit Signature";


                            // Add data to the EndUserManagement table
                            var endUserManagementData = _context.EndUserManagements.Where(i => i.UserGroup.CompanyAdminUser.AspId == claim.Value).ToList();
                            if (endUserManagementData != null && endUserManagementData.Count > 0)
                            {
                                for (int i = 0; i < endUserManagementData.Count; i++)
                                {
                                    wsEndUserManagement.Cells[i + 2, 1].Value = endUserManagementData[i].UserGroup.UserGroupName;
                                    wsEndUserManagement.Cells[i + 2, 2].Value = endUserManagementData[i].EditEmail;
                                    wsEndUserManagement.Cells[i + 2, 3].Value = endUserManagementData[i].EditPassword;
                                    wsEndUserManagement.Cells[i + 2, 4].Value = endUserManagementData[i].EditSign;
                                }
                            }

                            // Add "Document Management Permissions" worksheet
                            var wsDocumentManagement = package.Workbook.Worksheets.Add("Document Management Permissions");

                            // Add headers for the Document Management table                          
                            wsDocumentManagement.Cells[1, 1].Value = "Group Name";
                            wsDocumentManagement.Cells[1, 2].Value = "View Doc";
                            wsDocumentManagement.Cells[1, 3].Value = "Add Single Doc";
                            wsDocumentManagement.Cells[1, 4].Value = "Add Multiple Doc";
                            wsDocumentManagement.Cells[1, 5].Value = "Copy Doc";
                            wsDocumentManagement.Cells[1, 6].Value = "Move Doc";
                            wsDocumentManagement.Cells[1, 7].Value = "Delete Doc";
                            wsDocumentManagement.Cells[1, 8].Value = "Rename Doc";
                            wsDocumentManagement.Cells[1, 9].Value = "Private Doc";
                            wsDocumentManagement.Cells[1, 10].Value = "Download Doc";
                            wsDocumentManagement.Cells[1, 11].Value = "Print Doc";
                            wsDocumentManagement.Cells[1, 12].Value = "View Metadata";
                            wsDocumentManagement.Cells[1, 13].Value = "Edit Metadata";
                            wsDocumentManagement.Cells[1, 14].Value = "Share Doc Internal";
                            wsDocumentManagement.Cells[1, 15].Value = "Share Doc External";
                            wsDocumentManagement.Cells[1, 16].Value = "Share Signature External";
                            wsDocumentManagement.Cells[1, 17].Value = "Audit log Doc";
                            wsDocumentManagement.Cells[1, 18].Value = "Doc Version View";
                            wsDocumentManagement.Cells[1, 19].Value = "Doc RollBack";
                            wsDocumentManagement.Cells[1, 20].Value = "Download CSV Report";
                            wsDocumentManagement.Cells[1, 21].Value = "Audit log Users";
                            wsDocumentManagement.Cells[1, 22].Value = "Assign Doc Users";
                            wsDocumentManagement.Cells[1, 23].Value = "Max Doc Upload Size";
                            wsDocumentManagement.Cells[1, 24].Value = "Max Doc Upload Number";

                            // Add data to the Document Management table
                            var documentManagementData = _context.DocumentManagements.Where(i => i.UserGroup.CompanyAdminUser.AspId == claim.Value).ToList();
                            if (documentManagementData != null && documentManagementData.Count > 0)
                            {
                                for (int i = 0; i < documentManagementData.Count; i++)
                                {
                                    wsDocumentManagement.Cells[i + 2, 1].Value = documentManagementData[i].UserGroup.UserGroupName;
                                    wsDocumentManagement.Cells[i + 2, 2].Value = documentManagementData[i].ViewDoc;
                                    wsDocumentManagement.Cells[i + 2, 3].Value = documentManagementData[i].DocSinAdd;
                                    wsDocumentManagement.Cells[i + 2, 4].Value = documentManagementData[i].DocMulAdd;
                                    wsDocumentManagement.Cells[i + 2, 5].Value = documentManagementData[i].DocCopy;
                                    wsDocumentManagement.Cells[i + 2, 6].Value = documentManagementData[i].DocMove;
                                    wsDocumentManagement.Cells[i + 2, 7].Value = documentManagementData[i].DocDelete;
                                    wsDocumentManagement.Cells[i + 2, 8].Value = documentManagementData[i].DocRename;
                                    wsDocumentManagement.Cells[i + 2, 9].Value = documentManagementData[i].DocPrivate;
                                    wsDocumentManagement.Cells[i + 2, 10].Value = documentManagementData[i].DocDown;
                                    wsDocumentManagement.Cells[i + 2, 11].Value = documentManagementData[i].DocPrint;
                                    wsDocumentManagement.Cells[i + 2, 12].Value = documentManagementData[i].ViewMatadata;
                                    wsDocumentManagement.Cells[i + 2, 13].Value = documentManagementData[i].EditMatadata;
                                    wsDocumentManagement.Cells[i + 2, 14].Value = documentManagementData[i].ShareDocInt;
                                    wsDocumentManagement.Cells[i + 2, 15].Value = documentManagementData[i].ShareDocExt;
                                    wsDocumentManagement.Cells[i + 2, 16].Value = documentManagementData[i].ShareSigExt;
                                    wsDocumentManagement.Cells[i + 2, 17].Value = documentManagementData[i].AuditLogDoc;
                                    wsDocumentManagement.Cells[i + 2, 18].Value = documentManagementData[i].DocVerView;
                                    wsDocumentManagement.Cells[i + 2, 19].Value = documentManagementData[i].DocRollBack;
                                    wsDocumentManagement.Cells[i + 2, 20].Value = documentManagementData[i].DownCsvRpt;
                                    wsDocumentManagement.Cells[i + 2, 21].Value = documentManagementData[i].AuditLogUser;
                                    wsDocumentManagement.Cells[i + 2, 22].Value = documentManagementData[i].AsgnDocUser;
                                    wsDocumentManagement.Cells[i + 2, 23].Value = documentManagementData[i].MaxDocUpSize;
                                    wsDocumentManagement.Cells[i + 2, 24].Value = documentManagementData[i].MaxDocUpNum;
                                }
                            }

                            // Add "Document Management Permissions" worksheet
                            var wsRadManagement = package.Workbook.Worksheets.Add("RAD Management Permissions");

                            // Add headers for the Rad Management table                          
                            wsRadManagement.Cells[1, 1].Value = "Group Name";
                            wsRadManagement.Cells[1, 2].Value = "RAD View";
                            wsRadManagement.Cells[1, 3].Value = "RAD Edit";
                            wsRadManagement.Cells[1, 4].Value = "RAD Form Fill";

                            // Add data to the Rad Management table
                            var radManagementData = _context.RADManagements.Where(i => i.UserGroup.CompanyAdminUser.AspId == claim.Value).ToList();
                            if (radManagementData != null && radManagementData.Count > 0)
                            {
                                for (int i = 0; i < radManagementData.Count; i++)
                                {
                                    wsRadManagement.Cells[i + 2, 1].Value = radManagementData[i].UserGroup.UserGroupName;
                                    wsRadManagement.Cells[i + 2, 2].Value = radManagementData[i].RadView;
                                    wsRadManagement.Cells[i + 2, 3].Value = radManagementData[i].RadEdit;
                                    wsRadManagement.Cells[i + 2, 4].Value = radManagementData[i].RadFormFill;
                                }
                            }

                            // Save the Excel package to a stream
                            using (MemoryStream stream = new MemoryStream())
                            {
                                package.SaveAs(stream);

                                string filename = $"UserGroups_And_EndUserManagement_And_DocumentManagement_And_RADManagement{DateTime.Now.ToString("ddMMyyyy")}.xlsx";
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
            return RedirectToAction("Details");
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

                    var filteredUserGroups = userGroupList
                    .Where(u =>
                     _context.DocumentManagements.Any(d => d.GroupId == u.UserGroupId) ||
                    _context.EndUserManagements.Any(e => e.GroupId == u.UserGroupId) ||
                     _context.RADManagements.Any(r => r.GroupId == u.UserGroupId)
                    ).Select(u => new
                    {
                        UserId = u.UserGroupId,
                        GroupName = u.UserGroupName
                    });

                    userlist.AddRange(filteredUserGroups);
                }

                return Json(new { data = userlist });
            }
            return Ok();
        }
        #endregion
    }
}
