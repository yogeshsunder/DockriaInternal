using ClosedXML.Excel;
using Dockria.Data;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Model;
using Domain.Model.ViewModel;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace WebApp.Areas.CompanyAdmin.Controllers
{
    [Area("CompanyAdmin")]
    public class ContainerTypeController : Controller
    {

        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public ContainerTypeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private string GenerateRandomCode()
        {
            // Implement your logic to generate a unique random code
            // For example, you can use Guid.NewGuid() to generate a unique identifier
            return Guid.NewGuid().ToString();
        }


        [HttpGet]
        public IActionResult ContainerIndex(string id)
        {
            // Use the id parameter as needed in your CompanyAdmin action
            ViewBag.RandomString = id;

            // If id is not provided or is empty, generate a new random code
            if (string.IsNullOrEmpty(id))
            {
                string randomCode = GenerateRandomCode();
                ViewBag.RandomString = randomCode;
                return RedirectToAction("ContainerIndex", new { id = randomCode });
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BulkValueAddition(IFormFile formFile)
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
                    await formFile.CopyToAsync(stream)
;
                    using (var spreadsheetDocument = SpreadsheetDocument.Open(stream, false))
                    {
                        var sheet = spreadsheetDocument.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                        var worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(sheet.Id);
                        var rows = worksheetPart.Worksheet.Descendants<Row>();

                        var values = new List<ValueInfo>(); // Changed the variable name

                        // Create lists to collect success and error messages
                        var successMessages = new List<string>();
                        var errorMessages = new List<string>();

                        // Assuming data starts from row 2 (excluding header row)
                        foreach (var row in rows.Skip(1))
                        {
                            var cells = row.Elements<Cell>().ToList();

                            // Check if the cells in the row have non-empty data
                            bool hasData = cells.Any(cell => !string.IsNullOrEmpty(SD.GetCellValue(spreadsheetDocument, cell)));

                            if (!hasData)
                            {
                                continue; // Continue to the next row if there's no data
                            }

                            var value = new ValueInfo
                            {
                                Name = SD.GetCellValue(spreadsheetDocument, cells[0]),
                                // Add other properties as needed
                            };

                            values.Add(value);
                        }

                        if (values.Any())
                        {
                            _context.Values.AddRange(values);
                            await _context.SaveChangesAsync();

                            // Add success messages
                            foreach (var value in values)
                            {
                                successMessages.Add($"Value '{value.Name}' added successfully.");
                            }

                            return Json(new { success = true, successMessage = successMessages, errorMessage = errorMessages });
                        }
                        else
                        {
                            return Json(new { success = false, errorMessage = "No valid data found." });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;


                return Json(new { success = false, errors = new[] { errorMessage } });
            }
        }





        [HttpPost]
        public IActionResult Create(ContainerType containerType, string SelectedUserGroups, string SelectedMetadata, string SelectedValues)
        {
            try
            {
                // Deserialize the selected metadata
                var metaDataList = JsonConvert.DeserializeObject<List<ContainerMetaData>>(SelectedMetadata);

                // Deserialize the selected values
                var valueList = JsonConvert.DeserializeObject<List<ValueInfo>>(SelectedValues);

                // Ensure the deserialized lists are not null
                metaDataList ??= new List<ContainerMetaData>();
                valueList ??= new List<ValueInfo>();

                // Set the ContainerType property of each ContainerMetaData instance
                foreach (var metaData in metaDataList)
                {
                    metaData.ContainerType = containerType;
                    metaData.ContainerId = containerType.Id;
                }

                // Use the selected metadata list
                containerType.MetaDataList = metaDataList;

                foreach (var value in valueList)
                {
                    value.ContainerType = containerType;
                    value.ContainerId = containerType.Id;
                }
                containerType.Values = valueList;

                // Check if the container name already exists
                if (_context.ContainerTypes.Any(ct => ct.ContainerName == containerType.ContainerName))
                {
                    var errorMessage = containerType.ContainerName + " Already exists. Please select another Container Type Name...";
                    ModelState.AddModelError("ContainerName", errorMessage);
                    return Json(new { success = false, isExistContainerNameError = true, errorMessage = errorMessage });
                }


                if (ModelState.IsValid)
                {
                    var claimIdentity = (ClaimsIdentity)User.Identity;
                    var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                    if (claim != null)
                    {
                        var userId = claim.Value;
                        var isCompanyAdmin = User.IsInRole("Company Admin");

                        var user = _context.CompanyUsers.FirstOrDefault(cu => cu.AspId == userId);
                        var companyAdmin = isCompanyAdmin
                            ? _context.CompanyAdmin.FirstOrDefault(ca => ca.AspId == userId)
                            : (user != null ? _context.CompanyAdmin.Find(user.UserId) : null);

                        if (companyAdmin != null)
                        {
                            // Query the database to retrieve the corresponding UserGroup records
                            var selectedUserGroupIds = JsonConvert.DeserializeObject<List<int>>(SelectedUserGroups);
                            var userGroupList = _context.UserGroups.Where(group => selectedUserGroupIds.Contains(group.UserGroupId)).ToList();

                            // Add the selected UserGroups to the new MetaDataType instance
                            containerType.UserGroupNames = userGroupList.Select(userGroup => new ContainerUserGroup
                            {
                                UserGroupId = userGroup.UserGroupId,
                                UserGroup = userGroup
                            }).ToList();

                            containerType.CompanyAdminId = companyAdmin.Id;
                            containerType.CompanyAdminUser = companyAdmin;

                            // Do not specify the ValueInfo's identity column (if it has one) in the INSERT statement
                            // Here, I'm assuming your 'ValueInfo' class has an identity column named 'Id'.
                            // You should adapt this part based on your actual database schema.
                            containerType.Values.ForEach(valueData =>
                            {
                                valueData.Id = 0; // Set the identity column value to 0
                            });

                            _context.ContainerTypes.Add(containerType);
                            _context.SaveChanges();
                        }

                        var currentUser = _context.ApplicationUsers.FirstOrDefault(i => i.Id == userId);
                        var auditLog = new AuditLog
                        {
                            Username = currentUser.UserName,
                            Fullname = currentUser.FullName,
                            Time = DateTime.Now.TimeOfDay, // Save only the time part
                            Date = DateTime.Now.Date, // Save only the date part
                            BrowserName = HttpContext.Request.Headers["User-Agent"].ToString(),
                            IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                            //Action = "Container Type Created" + " " + currentUser.FullName,
                            RecordType = "Container Type Created" + " " + currentUser.FullName,
                            //OldData = "N/A",
                            NewData = $"New Document Created No {containerType.Id}",
                            //Description = $"Document created by {currentUser.FullName} Document No. {containerType.Id}"
                        };

                        _context.AuditLogs.Add(auditLog);
                        _context.SaveChanges();
                        return Json(new { success = true, message = "Container Type MetaData Added Successfully..." });
                    }

                    return Json(new { success = false });
                }
                else
                {
                    // Return error response with error messages if the form data is invalid
                    var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, errorMessage = "Validation failed.", errors = errorMessages });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                // Handle the error, return an appropriate error response, or log the error details
                return Json(new { success = false, errorMessage = "An error occurred while processing your request." });
            }
        }


        [HttpGet]
        public IActionResult Update(string id)
        {
            var containerType = _context.ContainerTypes
                .Include(ct => ct.UserGroupNames)
                .Include(ct => ct.MetaDataList)
                .FirstOrDefault(ct => ct.Id == id);

            if (containerType == null)
            {
                return Json(new { success = false, message = "Container Type MetaData Not Found...." });
            }

            var userGroups = _context.ContainerUserGroups
                .Where(cug => cug.ContainerTypeId == containerType.Id)
                .Select(cug => new
                {
                    Value = cug.UserGroupId,
                    Text = cug.UserGroup.UserGroupName
                }).ToList();

            // Retrieve selected values for the container type
            var selectedValues = _context.Values
                .Where(v => v.ContainerId == containerType.Id)
                .Select(v => new
                {
                    ValueId = v.ValueId,
                    Name = v.Name  // Ensure the property name matches your data model
                }).ToList();

            // Create a new object containing the ContainerType, userGroups, and selectedValues
            var data = new
            {
                id = containerType.Id,
                containerName = containerType.ContainerName,
                // DataType = containerType.ContainerDataType,
                metaDataList = containerType?.MetaDataList?.Select(metaData => new
                {
                    id = metaData.Id,
                    metaDataName = metaData.MetadataName,
                    Required = metaData.isRequired
                }).ToList(),
                UserGroups = userGroups,
                SelectedValues = selectedValues
            };

            return Json(new { success = true, Data = data });
        }


        [HttpPost]
        public IActionResult Update(ContainerType containerType, string UpdatedUserGroups, string UpdatedMetadata, string UpdatedValues)
        {
            var existContainerDoc = _context.ContainerTypes.Include(i => i.UserGroupNames).Include(i => i.MetaDataList)
                .Include(i => i.CompanyAdminUser).Where(i => i.Id == containerType.Id).FirstOrDefault();

            // Check if the container name is being updated
            if (existContainerDoc.ContainerName != containerType.ContainerName)
            {
                var existContainerName = _context.ContainerTypes
                    .Where(i => i.ContainerName == containerType.ContainerName)
                    .FirstOrDefault();

                if (existContainerName != null)
                {
                    return Json(new
                    {
                        isExistContainerNameError = true,
                        message = containerType.ContainerName + " Already exists. Please select another Container Type Name..."
                    });
                }
            }
            try
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                // Save a copy of the existing data in the audit log before making changes
                var previousData = JsonConvert.SerializeObject(existContainerDoc, serializerSettings);
                var UserGroups = JsonConvert.DeserializeObject<List<string>>(UpdatedUserGroups);
                if (UserGroups != null)
                {
                    var selectedUserGroupIds = UserGroups.Select(int.Parse).ToList();
                    var existingContainerTypeUserGroups = _context.ContainerUserGroups
                        .Where(i => i.ContainerTypeId == containerType.Id).ToList();

                    var existingUserGroupIds = existingContainerTypeUserGroups.Select(ug => ug.UserGroupId).ToList();

                    // Find user group IDs to remove (IDs present in existing but not in selected)
                    var userGroupIdsToRemove = existingUserGroupIds.Except(selectedUserGroupIds).ToList();

                    // Remove associations for user group IDs to remove
                    var associationsToRemove = existingContainerTypeUserGroups
                        .Where(association => userGroupIdsToRemove.Contains(association.UserGroupId))
                        .ToList();
                    _context.ContainerUserGroups.RemoveRange(associationsToRemove);

                    // Find user group IDs to add (IDs present in selected but not in existing)
                    var newUserGroupIdsToAdd = selectedUserGroupIds.Except(existingUserGroupIds).ToList();

                    // Retrieve the user groups to add based on IDs
                    var userGroupsToAdd = _context.UserGroups
                        .Where(group => newUserGroupIdsToAdd.Contains(group.UserGroupId))
                        .ToList();

                    // Create and add new associations for user groups to add
                    foreach (var userGroup in userGroupsToAdd)
                    {
                        var newAssociation = new ContainerUserGroup
                        {
                            ContainerTypeId = containerType.Id,
                            ContainerType = _context.ContainerTypes.Find(containerType.Id),
                            UserGroupId = userGroup.UserGroupId,
                            UserGroup = userGroup
                        };

                        _context.ContainerUserGroups.Add(newAssociation);
                    }

                    var valueList = JsonConvert.DeserializeObject<List<ValueInfo>>(UpdatedValues);

                    if (valueList != null)
                    {
                        // Fetch the existing ContainerType entity including its related Values
                        var existingContainerType = _context.ContainerTypes
                            .Include(c => c.Values)
                            .FirstOrDefault(c => c.Id == containerType.Id);

                        if (existingContainerType != null)
                        {
                            // Update the basic properties of the ContainerType
                            existingContainerType.ContainerName = containerType.ContainerName;
                            // existingContainerType.ContainerDataType = containerType.ContainerDataType;

                            // Iterate through valueList and update existing values
                            foreach (var newValue in valueList)
                            {
                                var existingValue = existingContainerType.Values.FirstOrDefault(v => v.ValueId == newValue.ValueId);
                                if (existingValue != null)
                                {
                                    // Update existing value properties
                                    existingValue.Name = newValue.Name;
                                    // Update any other properties as needed
                                }
                                else
                                {
                                    // If the value doesn't exist, it might be a new one, so add it
                                    newValue.ContainerType = existingContainerType;
                                    existingContainerType.Values.Add(newValue);
                                }
                            }

                            // No need to clear the existing values

                            // Save changes to the database
                            _context.SaveChanges();
                        }
                    }
                }

                var metaDataList = JsonConvert.DeserializeObject<List<ContainerMetaData>>(UpdatedMetadata);

                if (metaDataList != null)
                {
                    if (ModelState.IsValid)
                    {
                        if (existContainerDoc != null)
                        {
                            // Remove existing metadata items
                            _context.ContainerMetaDatas.RemoveRange(existContainerDoc.MetaDataList);

                            // Add new metadata items from metaDataList
                            foreach (var updatedMetadata in metaDataList)
                            {
                                // Ensure that you associate the metadata with the current container type
                                updatedMetadata.ContainerId = containerType.Id;
                                existContainerDoc.MetaDataList.Add(updatedMetadata);
                            }

                            var claimIdentity = (ClaimsIdentity)User.Identity;
                            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                            if (claim != null)
                            {
                                var userId = claim.Value;
                                var currentUser = _context.ApplicationUsers.FirstOrDefault(i => i.Id == userId);
                                if (currentUser != null)
                                {
                                    existContainerDoc.ContainerName = containerType.ContainerName;
                                    // existContainerDoc.ContainerDataType = containerType.ContainerDataType;

                                    // Save changes to the database
                                    _context.SaveChanges();

                                    var auditLog = new AuditLog
                                    {
                                        Username = currentUser.UserName,
                                        Fullname = currentUser.FullName,
                                        Time = DateTime.Now.TimeOfDay, // Save only the time part
                                        Date = DateTime.Now.Date, // Save only the date part
                                        BrowserName = HttpContext.Request.Headers["User-Agent"].ToString(),
                                        IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                                        //Action = "Container Type Updated" + " " + currentUser.FullName,
                                        RecordType = "Container Type Updated" + " " + containerType.Id,
                                        //OldData = previousData,
                                        NewData = $"Container Type Updated No {containerType.Id}",
                                        //Description = $"Container Type Updated by {currentUser.FullName} Container Type No." + " " +
                                        //  $" {containerType.Id} Previous Container Type MetaData" + " " + previousData
                                    };

                                    _context.AuditLogs.Add(auditLog);
                                    _context.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            // If metaDataList is empty, add a default metadata entry with the current datetime
                            containerType.MetaDataList = new List<ContainerMetaData> { new ContainerMetaData { MetadataName = DateTime.Now.ToString() } };
                        }
                        return Json(new { success = true, message = "Container Type MetaData Updated Successfully..." });
                    }
                }

                return Json(new { success = false, message = "Some error occurred while updating Container Type MetaData. Please try again later." });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.ToString() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportMetaData(IFormFile formFile)
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
                var aspUser = await _context.CompanyUsers.FirstOrDefaultAsync(i => i.UserId == claim.Value);
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

                        var metaDataList = new List<MetaDataType>();

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
                            var metaData = new MetaDataType
                            {
                                MetaDataTypeName = SD.GetCellValue(spreadsheetDocument, cells[0]),
                                MetaDataDataType = "Text",
                                // Set the CompanyAdminId and CompanyAdminUser properties from the aspUser object
                                CompanyAdminId = Convert.ToInt32(aspUser.UserId),
                                CompanyAdminUser = await _context.CompanyAdmin.FindAsync(aspUser.UserId)
                            };

                            metaDataList.Add(metaData);
                        }

                        // Save all metadata to the database
                        _context.MetaDataTypes.AddRange(metaDataList);
                        await _context.SaveChangesAsync();
                    }
                }
                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public IActionResult ShowMetaDataForm()
        {
            return PartialView("_showMetaDataForm");
        }

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

        //Download Excel file
        public IActionResult ExportExcel()
        {
            try
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    var data = _context.ContainerTypes.Include(i => i.CompanyAdminUser)
                       .Include(i => i.MetaDataList).Include(i => i.UserGroupNames).ThenInclude(ug => ug.UserGroup)
                       .Where(c => c.CompanyAdminUser.AspId == claim.Value).ToList();
                    if (data != null && data.Count > 0)
                    {
                        // Select only the "GroupName" and "UserName" columns from the data
                        var filteredData = data.Select(group => new
                        {
                            containerName = group.ContainerName,
                            //  containerType = group.ContainerDataType,
                            GroupName = group.UserGroupNames,
                            MetaDataName = group.MetaDataList
                        }).ToList();

                        if (filteredData.Count > 0)
                        {
                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                var ws = wb.Worksheets.Add("Container Type");

                                // Add table headers and format them
                                var headerRow = ws.Row(1); // Select the first row (header row)
                                headerRow.Style.Font.Bold = true; // Make the text bold
                                headerRow.Style.Font.FontSize = 12; // Set the font size

                                // Add table headers
                                ws.Cell(1, 1).Value = "Container Name";
                                ws.Cell(1, 2).Value = "Container Type";
                                ws.Cell(1, 3).Value = "Group Name";
                                ws.Cell(1, 4).Value = "MetaData Name";

                                // Add data to the table
                                for (int i = 0; i < filteredData.Count; i++)
                                {
                                    ws.Cell(i + 2, 1).Value = filteredData[i].containerName;
                                    // ws.Cell(i + 2, 2).Value = filteredData[i].containerType;

                                    string groupNames = string.Join(", ", filteredData[i].GroupName.Select(ug => ug.UserGroup?.UserGroupName));
                                    ws.Cell(i + 2, 3).Value = groupNames;

                                    string metadataName = string.Join(", ", filteredData[i].MetaDataName.Select(ug => ug.MetadataName));
                                    ws.Cell(i + 2, 4).Value = groupNames;
                                }

                                using (MemoryStream stream = new MemoryStream())
                                {
                                    wb.SaveAs(stream);
                                    string filename = $"ContainerType_{DateTime.Now.ToString("ddMMyyyy")}.xlsx";
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

        public IActionResult ExportPdf()
        {
            try
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    var data = _context.ContainerTypes.Include(i => i.CompanyAdminUser)
                        .Include(i => i.MetaDataList).Include(i => i.UserGroupNames).ThenInclude(ug => ug.UserGroup)
                        .Where(c => c.CompanyAdminUser.AspId == claim.Value).ToList();
                    if (data != null && data.Count > 0)
                    {
                        // Select only the "GroupName" and "UserName" columns from the data
                        var filteredData = data.Select(group => new
                        {
                            ContainerName = group.ContainerName,
                            //  ContainerType = group.ContainerDataType,
                            GroupName = group.UserGroupNames,
                            MetaDataName = group.MetaDataList
                        }).ToList();

                        if (filteredData.Count > 0)
                        {
                            using (PdfDocument document = new PdfDocument())
                            {
                                PdfPage page = document.AddPage();
                                page.Size = PageSize.A4;
                                page.Orientation = PageOrientation.Portrait;

                                XGraphics gfx = XGraphics.FromPdfPage(page);

                                // Define fonts
                                XFont font = new XFont("Verdana", 10, XFontStyle.Regular);
                                XFont boldFont = new XFont("Arial", 14, XFontStyle.Bold);

                                // Define the starting position of the table
                                double x = 50;
                                double y = 50;
                                double rowMargin = 10; // Add some space between rows
                                // Initialize column widths
                                double containerNameWidth = 0;
                                double containerTypeWidth = 0;
                                double groupNameWidth = 0;
                                double metaDataNameWidth = 0;

                                // Add table headers and calculate column widths
                                gfx.DrawString("Container Name", boldFont, XBrushes.Black, new XPoint(x, y));
                                containerNameWidth = Math.Max(containerNameWidth, gfx.MeasureString("Container Name", boldFont).Width);

                                gfx.DrawString("Container Type", boldFont, XBrushes.Black, new XPoint(x + containerNameWidth + 10, y));
                                containerTypeWidth = Math.Max(containerTypeWidth, gfx.MeasureString("Container Type", boldFont).Width);

                                gfx.DrawString("Group Name", boldFont, XBrushes.Black, new XPoint(x + containerNameWidth + containerTypeWidth + 20, y));
                                groupNameWidth = Math.Max(groupNameWidth, gfx.MeasureString("Group Name", boldFont).Width);

                                gfx.DrawString("MetaData Name", boldFont, XBrushes.Black, new XPoint(x + containerNameWidth + containerTypeWidth + groupNameWidth + 30, y));
                                metaDataNameWidth = Math.Max(metaDataNameWidth, gfx.MeasureString("MetaData Name", boldFont).Width);

                                y += boldFont.Height + rowMargin;

                                // Add data to the table
                                for (int i = 0; i < filteredData.Count; i++)
                                {
                                    // Calculate the maximum row height based on column content
                                    double rowHeight = 0;

                                    // Calculate the height needed for container name
                                    var containerNameSize = gfx.MeasureString(filteredData[i].ContainerName, font);
                                    rowHeight = Math.Max(rowHeight, containerNameSize.Height);

                                    // Calculate the height needed for container type
                                    // var containerTypeSize = gfx.MeasureString(filteredData[i].ContainerType, font);
                                    //  rowHeight = Math.Max(rowHeight, containerTypeSize.Height);

                                    // Calculate the height needed for group names
                                    double groupHeight = 0;
                                    foreach (var groups in filteredData[i].GroupName)
                                    {
                                        var groupSize = gfx.MeasureString(groups.UserGroup.UserGroupName, font);
                                        groupHeight += groupSize.Height;
                                    }
                                    rowHeight = Math.Max(rowHeight, groupHeight);

                                    // Calculate the height needed for metadata values
                                    double metadataHeight = 0;
                                    foreach (var metaData in filteredData[i].MetaDataName)
                                    {
                                        var metadataSize = gfx.MeasureString(metaData.MetadataName, font);
                                        metadataHeight += metadataSize.Height;
                                    }
                                    rowHeight = Math.Max(rowHeight, metadataHeight);

                                    // Draw each column's data with proper alignment
                                    gfx.DrawString(filteredData[i].ContainerName, font, XBrushes.Black, new XPoint(x, y));
                                    //  gfx.DrawString(filteredData[i].ContainerType, font, XBrushes.Black, new XPoint(x + containerNameWidth + 10, y));

                                    double currentY = y;
                                    foreach (var groups in filteredData[i].GroupName)
                                    {
                                        gfx.DrawString(groups.UserGroup.UserGroupName, font, XBrushes.Black, new XPoint(x + containerNameWidth + containerTypeWidth + 20, currentY));
                                        currentY += gfx.MeasureString(groups.UserGroup.UserGroupName, font).Height;
                                    }

                                    currentY = y;
                                    foreach (var metaData in filteredData[i].MetaDataName)
                                    {
                                        gfx.DrawString(metaData.MetadataName, font, XBrushes.Black, new XPoint(x + containerNameWidth + containerTypeWidth + groupNameWidth + 30, currentY));
                                        currentY += gfx.MeasureString(metaData.MetadataName, font).Height;
                                    }

                                    // Update the y coordinate for the next row based on the maximum height
                                    y += rowHeight + rowMargin;
                                }

                                using (MemoryStream stream = new MemoryStream())
                                {
                                    document.Save(stream, false);
                                    string filename = $"ContainerType_{DateTime.Now.ToString("ddMMyyyy")}.pdf";
                                    stream.Position = 0;
                                    return File(stream.ToArray(), "application/pdf", filename);
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






        [HttpPost]
        public async Task<IActionResult> SaveValues([FromBody] ValueInfo data)
        {
            if (data == null)
            {
                return BadRequest("No data received.");
            }

            try
            {
                // Add the received values to the database
                _context.Values.Add(data);

                // Save changes to the database
                await _context.SaveChangesAsync();
                var values = _context.Values.ToList(); // Assuming you have a DbContext named context

                // Map your domain model to a simpler DTO if needed
                var result = values.Select(v => new { id = v.Id, name = v.Name, valueId = v.ValueId }).ToList();

                return Json(new { success = true, data = result, message = "Data saved successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        //[HttpPost]
        //public async Task<IActionResult> SaveValues([FromBody] ValueInfo data, string containerId)
        //{
        //    if (data == null || string.IsNullOrEmpty(containerId))
        //    {
        //        return BadRequest("Invalid data or containerId.");
        //    }

        //    try
        //    {
        //        // Retrieve the container based on containerId
        //        var container = await _context.ContainerTypes
        //            .Include(c => c.Values)
        //            .FirstOrDefaultAsync(c => c.Id == containerId);

        //        if (container == null)
        //        {
        //            return NotFound("Container not found.");
        //        }

        //        // Associate the value with the container
        //        data.ContainerId = containerId; // Assuming containerId is a string
        //        container.Values.Add(data);

        //        // Save changes to the database
        //        await _context.SaveChangesAsync();

        //        // Retrieve values associated with the specified containerId
        //        var values = _context.Values
        //            .Where(v => v.ContainerId == containerId)
        //            .ToList();

        //        // You may want to map the values to a DTO if needed
        //        var result = values.Select(v => new
        //        {
        //            Id = v.Id,
        //            Name = v.Name,
        //            ValueId = v.ValueId,
        //        }).ToList();

        //        return Json(new { success = true, data = result, message = "Data saved successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest($"Error: {ex.Message}");
        //    }
        //}


        [HttpGet]
        public async Task<IActionResult> EditValue(int id) // Change the parameter type to int
        {
            try
            {
                // Fetch the data based on the provided ID (id)

                var value = await _context.Values.FindAsync(id)
;

                if (value == null)
                {
                    return NotFound();
                }

                // Return the data as JSON
                return Json(new { Id = value.Id, Name = value.Name }); // Use 'Id' property as the key
            }
            catch (Exception ex)
            {
                // Log the exception or return an error response
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult EditValue(int id, string editedValue)
        {
            try
            {
                // Fetch the record based on the provided ID
                var value = _context.Values.FirstOrDefault(v => v.Id == id);

                if (value == null)
                {
                    return NotFound();
                }

                // Update the value with the editedValue
                value.Name = editedValue;

                // Save changes to the database
                _context.SaveChanges();

                // Return a success message
                return Ok("Value updated successfully.");
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the update
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult DeleteValues([FromBody] List<ValueInfo> deletedValues)
        {
            try
            {
                // Extract the ValueIds from the deletedValues list
                var valueIdsToDelete = deletedValues.Select(v => v.ValueId).ToList();

                // Find the values in the database
                var valuesToDelete = _context.Values.Where(v => valueIdsToDelete.Contains(v.ValueId)).ToList();

                // Remove the values from the database
                _context.Values.RemoveRange(valuesToDelete);

                // Save changes
                _context.SaveChanges();

                // Return a success response
                return Json(new { success = true, message = "Values deleted successfully." });
            }
            catch (Exception ex)
            {
                // Handle exception
                return Json(new { success = false, message = "Error deleting values. Please try again." });
            }
        }


        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var aspUser = _context.ApplicationUsers.FirstOrDefault(i => i.Id == claim.Value);
                if (aspUser != null)
                {
                    var containerTypeList = _context.ContainerTypes.Where(i => i.CompanyAdminUser.AspId == aspUser.Id).Select(i => new
                    {
                        id = i.Id,
                        containerName = i.ContainerName,
                        UserGroup = i.UserGroupNames,
                        // containerDataType = i.ContainerDataType
                    }).ToList();

                    return Json(new { success = "true", data = containerTypeList });
                }
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public IActionResult GetAllContainerTypeMetadata()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var aspUser = _context.ApplicationUsers.FirstOrDefault(i => i.Id == claim.Value);
                if (aspUser != null)
                {
                    var docTypeMetaDataList = _context.MetaDataTypes.Where(i => i.CompanyAdminId == Convert.ToInt32(aspUser.Id)).ToList();
                    return Json(new { success = "true", data = docTypeMetaDataList });
                }

            }
            return Json(new { success = false });
        }

        [HttpGet]
        public IActionResult GenerateUniqueId()
        {
            Guid uniqueId = Guid.NewGuid();
            return Json(uniqueId.ToString());
        }


        [HttpGet]
        public IActionResult GetValuesForContainer(string containerId)
        {
            var valuesForContainer = _context.Values
                .Where(v => v.ContainerId == containerId)
                .Select(v => new
                {
                    id = v.Id,
                    valueId = v.ValueId,
                    name = v.Name,
                    containerId = v.ContainerId,
                })
                .ToList();

            return Json(new { success = true, valuesForContainer });
        }



        [HttpDelete]
        public IActionResult DeleteContainer(string id)
        {
            var containerTypeDoc = _context.ContainerTypes
                .Include(i => i.MetaDataList)
                .Include(i => i.CompanyAdminUser)
                .Include(i => i.UserGroupNames)
                .Include(i => i.Values) // Include the related values
                .Where(i => i.Id == id)
                .FirstOrDefault();

            if (containerTypeDoc != null)
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

                if (claim != null)
                {
                    try
                    {
                        var serializerSettings = new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        };

                        // Save a copy of the existing data in the audit log before making changes
                        var deletedData = JsonConvert.SerializeObject(containerTypeDoc, serializerSettings);

                        var userId = claim.Value;
                        var currentUser = _context.ApplicationUsers.FirstOrDefault(i => i.Id == userId);

                        // Remove related values
                        _context.Values.RemoveRange(containerTypeDoc.Values);

                        // Remove other related entities
                        var containerMetaData = _context.ContainerMetaDatas.Where(i => i.ContainerId == containerTypeDoc.Id).ToList();
                        if (containerMetaData.Any())
                        {
                            _context.ContainerMetaDatas.RemoveRange(containerMetaData);
                        }

                        var containerUserGroups = _context.ContainerUserGroups.Where(i => i.ContainerTypeId == containerTypeDoc.Id).ToList();
                        if (containerUserGroups.Any())
                        {
                            _context.ContainerUserGroups.RemoveRange(containerUserGroups);
                        }

                        _context.ContainerTypes.Remove(containerTypeDoc);
                        _context.SaveChanges();

                        var auditLog = new AuditLog
                        {
                            Username = currentUser.UserName,
                            Fullname = currentUser.FullName,
                            Time = DateTime.Now.TimeOfDay,
                            Date = DateTime.Now.Date,
                            BrowserName = HttpContext.Request.Headers["User-Agent"].ToString(),
                            IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                            //Action = "Container Type MetaData Deleted By" + " " + currentUser.FullName,
                            RecordType = "Container Type MetaData Deleted By" + " " + currentUser.FullName,
                            //OldData = deletedData,
                            NewData = "NA",
                            //Description = $"Container Type MetaData Deleted By {currentUser.FullName} " + deletedData
                        };

                        _context.AuditLogs.Add(auditLog);
                        _context.SaveChanges();

                        return Json(new { success = true, message = "Container Type MetaData Deleted Successfully..." });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
            }

            return Json(new { success = false, message = "Container Type MetaData not found..." });
        }



        [HttpGet]
        public IActionResult GetAllValues()
        {
            var values = _context.Values.ToList(); // Assuming you have a DbContext named context

            // Map your domain model to a simpler DTO if needed
            var result = values.Select(v => new { Id = v.Id, Name = v.Name, ValueId = v.ValueId }).ToList();

            return Json(result);
        }

        #endregion
    }
}