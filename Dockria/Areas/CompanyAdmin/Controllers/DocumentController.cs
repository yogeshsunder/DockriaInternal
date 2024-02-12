using Dockria.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Model;
using PdfSharpCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using ClosedXML.Excel;
using PageSize = PdfSharpCore.PageSize;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text;
using DocumentFormat.OpenXml.ExtendedProperties;
using System.Linq;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace WebApp.Areas.CompanyAdmin.Controllers
{
    [Area("CompanyAdmin")]
    public class DocumentController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public DocumentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        private string GenerateRandomCode()
        {
            // Implement your logic to generate a unique random code
            // For example, you can use Guid.NewGuid() to generate a unique identifier
            return Guid.NewGuid().ToString();
        }


        [HttpGet]
        public IActionResult DocumentMetaDataIndex(string id)
        {
            // Use the id parameter as needed in your CompanyAdmin action
            ViewBag.RandomString = id;

            // If id is not provided or is empty, generate a new random code
            if (string.IsNullOrEmpty(id))
            {
                string randomCode = GenerateRandomCode();
                ViewBag.RandomString = randomCode;
                return RedirectToAction("DocumentMetaDataIndex", new { id = randomCode });
            }

            return View();
        }

        [HttpPost]
        public IActionResult CreateDocument(Documents documents, string SelectedUserGroups, string SelectedMetadata)
        {
            var metaDataList = JsonConvert.DeserializeObject<List<MetaData>>(SelectedMetadata);
            if (metaDataList != null)
            {
                if (metaDataList.Count > 0)
                {
                    // Use the selected metadata list
                    documents.MetaDataList = metaDataList;
                }
                else
                {
                    // If metaDataList is empty, use the default metadata list
                    // Fetch the default metadata list from the database and assign it to the Documents object
                    // Replace 'YourDefaultMetadataList' with the actual method or query to fetch the default metadata list
                    //documents.MetaDataList = YourDefaultMetadataList();
                }
            }

            var UserGroups = JsonConvert.DeserializeObject<List<string>>(SelectedUserGroups);
            if (UserGroups != null)
            {
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
                            var selectedUserGroupIds = UserGroups.Select(int.Parse).ToList();
                            var userGroupList = _context.UserGroups.Where(group => selectedUserGroupIds.Contains(group.UserGroupId)).ToList();
                            // Add the selected UserGroups to the new MetaDataType instance
                            foreach (var userGroup in userGroupList)
                            {
                                documents.UserGroupNames.Add(new DocumentsUserGroup
                                {
                                    UserGroupId = userGroup.UserGroupId,
                                    UserGroup = userGroup
                                });
                            }

                            documents.CompanyAdminId = companyAdmin.Id;
                            documents.CompanyAdminUser = companyAdmin;
                        }

                        var autoFileName = _context.FileNameTypeDocuments.Find(documents.FileNameTypeId);
                        if (autoFileName != null)
                        {
                            documents.FileNameTypeId = autoFileName.Id;
                            documents.FileNameTypeDocument = autoFileName;
                        }
                        _context.Documents.Add(documents);
                        _context.SaveChanges();

                        var currentUser = _context.ApplicationUsers.FirstOrDefault(i => i.Id == userId);
                        var auditLog = new AuditLog
                        {
                            Username = currentUser.UserName,
                            Fullname = currentUser.FullName,
                            Time = DateTime.Now.TimeOfDay, // Save only the time part
                            Date = DateTime.Now.Date, // Save only the date part
                            BrowserName = HttpContext.Request.Headers["User-Agent"].ToString(),
                            IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                            //Action = "DocumentCreated",
                            RecordType = "DocumentCreated",
                            //  OldData = "N/A",
                            NewData = $"New Document Created No {documents.Id}",
                            //Description = $"Document created by {currentUser.FullName} Document No. {documents.Id}"
                        };

                        _context.AuditLogs.Add(auditLog);
                        _context.SaveChanges();
                    }
                    return Json(new { success = true, message = "Document Type MetaData Created Successfully..." });
                }
                else
                {
                    // Return error response with error messages if the form data is invalid
                    var errorMessages = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    return Json(new { success = false, errorMessage = "Validation failed.", errors = errorMessages });
                }
            }
            return Json(new { success = false, errorMessage = "Validation failed." });
        }

        [HttpGet]
        public IActionResult UpdateDocument(int id)
        {
            var documentType = _context.Documents.Include(i => i.UserGroupNames).Include(i => i.FileNameTypeDocument)
                            .Include(ct => ct.MetaDataList).FirstOrDefault(i => i.Id == id);
            if (documentType == null) return Json(new { success = false, message = "Document Type MetaData Not Found...." });
            var userGroups = _context.DocumentsUserGroups.Where(i => i.DocumentsId == documentType.Id)
                     .Select(u => new
                     {
                         Value = u.UserGroupId,
                         Text = u.UserGroup.UserGroupName
                     }).ToList();
            // Create a new object containing the ContainerType and userGroups
            var data = new
            {
                data = new
                {
                    id = documentType.Id,
                    docTypeName = documentType.DocTypeName,
                    ocr = documentType.OCR,
                    Versioning = documentType.VERSIONING,
                    autoFileName = _context.FileNameTypeDocuments.Where(n => n.Id == documentType.FileNameTypeId)
                    .Select(i => new
                    {
                        Value = i.Id,
                        Text = i.FileName,
                    }).ToList(),
                    metaDataList = documentType?.MetaDataList?.Select(metaData => new
                    {
                        id = metaData.Id,
                        metaDataName = metaData.MetadataName,
                        Required = metaData.isRequired
                    }).ToList(),
                    UserGroups = userGroups
                }
            };
            return Json(new { success = true, Data = data });
        }

        [HttpPost]
        public IActionResult UpdateDocument(Documents documents, string UpdatedUserGroups, string UpdatedMetaData)
        {
            var existDocument = _context.Documents.Include(i => i.UserGroupNames).Include(i => i.FileNameTypeDocument)
                            .Include(ct => ct.MetaDataList).FirstOrDefault(i => i.Id == documents.Id);
            try
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                // Save a copy of the existing data in the audit log before making changes
                var previousData = JsonConvert.SerializeObject(existDocument, serializerSettings);
                var UserGroups = JsonConvert.DeserializeObject<List<string>>(UpdatedUserGroups);
                if (UserGroups != null)
                {
                    var selectedUserGroupIds = UserGroups.Select(int.Parse).ToList();
                    var existDocumentUserGroups = _context.DocumentsUserGroups
                        .Where(i => i.DocumentsId == documents.Id).ToList();

                    var existingUserGroupIds = existDocumentUserGroups.Select(ug => ug.UserGroupId).ToList();

                    // Find user group IDs to remove (IDs present in existing but not in selected)
                    var userGroupIdsToRemove = existingUserGroupIds.Except(selectedUserGroupIds).ToList();

                    // Remove associations for user group IDs to remove
                    var associationsToRemove = existDocumentUserGroups
                        .Where(association => userGroupIdsToRemove.Contains(association.UserGroupId))
                        .ToList();
                    _context.DocumentsUserGroups.RemoveRange(associationsToRemove);

                    // Find user group IDs to add (IDs present in selected but not in existing)
                    var newUserGroupIdsToAdd = selectedUserGroupIds.Except(existingUserGroupIds).ToList();

                    // Retrieve the user groups to add based on IDs
                    var userGroupsToAdd = _context.UserGroups
                        .Where(group => newUserGroupIdsToAdd.Contains(group.UserGroupId))
                        .ToList();

                    // Create and add new associations for user groups to add
                    foreach (var userGroup in userGroupsToAdd)
                    {
                        var newAssociation = new DocumentsUserGroup
                        {
                            DocumentsId = documents.Id,
                            Documents = _context.Documents.Find(documents.Id),
                            UserGroupId = userGroup.UserGroupId,
                            UserGroup = userGroup
                        };

                        _context.DocumentsUserGroups.Add(newAssociation);
                    }

                    // Save changes to the database
                    _context.SaveChanges();
                }
                var metaDataList = JsonConvert.DeserializeObject<List<MetaData>>(UpdatedMetaData);
                if (metaDataList != null)
                {
                    if (ModelState.IsValid)
                    {
                        if (metaDataList.Count > 0)
                        {
                            if (existDocument != null)
                            {
                                var existingMetaDataList = existDocument.MetaDataList;
                                // Update existing metadata items and remove extra items
                                for (int i = 0; i < existingMetaDataList.Count; i++)
                                {
                                    if (i < metaDataList.Count)
                                    {
                                        existingMetaDataList[i].DocumentsId = documents.Id;
                                        existingMetaDataList[i].MetadataName = metaDataList[i].MetadataName;
                                        existingMetaDataList[i].isRequired = metaDataList[i].isRequired;
                                    }
                                    else
                                    {
                                        _context.MetaData.Remove(existingMetaDataList[i]);
                                    }
                                }
                                // Add new metadata items if needed
                                for (int i = existingMetaDataList.Count; i < metaDataList.Count; i++)
                                {
                                    metaDataList[i].DocumentsId = existDocument.Id;
                                    metaDataList[i].Documents = existDocument;
                                    existingMetaDataList.Add(metaDataList[i]);
                                }

                                var claimIdentity = (ClaimsIdentity)User.Identity;
                                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                                if (claim != null)
                                {
                                    var userId = claim.Value;
                                    var currentUser = _context.ApplicationUsers.FirstOrDefault(i => i.Id == userId);
                                    if (currentUser != null)
                                    {
                                        existDocument.DocTypeName = documents.DocTypeName;
                                        existDocument.OCR = documents.OCR;
                                        existDocument.VERSIONING = documents.VERSIONING;
                                        var fileTypeDocument = _context.FileNameTypeDocuments.Find(documents.FileNameTypeId);
                                        if (fileTypeDocument != null)
                                        {
                                            existDocument.FileNameTypeId = fileTypeDocument.Id;
                                            existDocument.FileNameTypeDocument = fileTypeDocument;
                                        }
                                        //if (autoFileName != null) existDocument.AutoFileName = autoFileName.FileName;
                                        existDocument.MetaDataList = metaDataList;
                                        _context.Documents.Update(existDocument);
                                        // Save changes to the database
                                        _context.SaveChanges();
                                    }
                                    var auditLog = new AuditLog
                                    {
                                        Username = currentUser.UserName,
                                        Fullname = currentUser.FullName,
                                        Time = DateTime.Now.TimeOfDay, // Save only the time part
                                        Date = DateTime.Now.Date, // Save only the date part
                                        BrowserName = HttpContext.Request.Headers["User-Agent"].ToString(),
                                        IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                                        //Action = "Document Type Updated" + " " + currentUser.FullName,
                                        RecordType = "Document Type Updated" + " " + documents.Id,
                                        //OldData = previousData,
                                        NewData = $"Document Type Updated No {existDocument.Id}",
                                        //Description = $"Document Type Updated by {currentUser.FullName} Document Type No." + " " +
                                        // $" {existDocument.Id} Previus Container Type MetaData" + " " + previousData
                                    };

                                    _context.AuditLogs.Add(auditLog);
                                    _context.SaveChanges();
                                }
                            }
                        }
                        return Json(new { success = true, message = $"Document Type '{existDocument.DocTypeName}' Updated Successfully..." });
                    }
                }
                return Json(new { success = false, message = "Some error occurred while updating Document Type MetaData. Please try again later." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        [HttpPost]
        public IActionResult CreateMetaDataType(MetaDataType metaData, string SelectedUserGroups)
        {
            var UserGroups = JsonConvert.DeserializeObject<List<string>>(SelectedUserGroups);
            if (UserGroups != null)
            {
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
                            var selectedUserGroupIds = UserGroups.Select(int.Parse).ToList();
                            var userGroupList = _context.UserGroups.Where(group => selectedUserGroupIds.Contains(group.UserGroupId)).ToList();
                            // Add the selected UserGroups to the new MetaDataType instance
                            foreach (var userGroup in userGroupList)
                            {
                                metaData.UserGroupNames.Add(new MetaDataTypeUserGroup
                                {
                                    UserGroupId = userGroup.UserGroupId,
                                    UserGroup = userGroup
                                });
                            }

                            metaData.CompanyAdminId = companyAdmin.Id;
                            metaData.CompanyAdminUser = companyAdmin;
                            _context.MetaDataTypes.Add(metaData);
                            _context.SaveChanges();

                            var currentUser = _context.ApplicationUsers.Find(userId);
                            var auditLog = new AuditLog
                            {
                                Username = currentUser.UserName,
                                Fullname = currentUser.FullName,
                                Time = DateTime.Now.TimeOfDay, // Save only the time part
                                Date = DateTime.Now.Date, // Save only the date part
                                BrowserName = HttpContext.Request.Headers["User-Agent"].ToString(),
                                IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                                //Action = "MetaData Type Created",
                                RecordType = "MetaData Type Created",
                                //OldData = "N/A",
                                NewData = $"New MetaData Type Created No {metaData.Id}",
                                //Description = $"New MetaData Type Created by {currentUser.FullName} MetaData No. {metaData.Id}"
                            };

                            _context.AuditLogs.Add(auditLog);
                            _context.SaveChanges();

                            return Json(new { success = true, message = "MetaData Type Added Successfully..." });
                        }
                    }
                    return Json(new { success = false, message = "Something wrong while added Metadata..." });
                }
            }

            return View();
        }

        [HttpGet]
        public IActionResult EditMetaDataType(int id)
        {
            var metaDataType = _context.MetaDataTypes.Include(i => i.UserGroupNames).FirstOrDefault(i => i.Id == id);
            if (metaDataType == null) return Json(new { success = false, message = "MetaData Type Document Not Found...." });
            var userGroups = _context.MetaDataTypeUserGroups.Where(i => i.MetaDataTypeId == metaDataType.Id)
                .Select(u => new
                {
                    Value = u.UserGroupId,
                    Text = u.UserGroup.UserGroupName
                }).ToList();

            // Create a new object containing the metaDataType and userGroups
            var data = new
            {
                success = true,
                data = new
                {
                    id = metaDataType.Id,
                    metaDataName = metaDataType.MetaDataTypeName,
                    metaDataType = metaDataType.MetaDataDataType,
                    UserGroups = userGroups
                }
            };
            return Json(data);
        }

        [HttpPost]
        public IActionResult UpdateMetaDataType(MetaDataType metaData, string SelectedUserGroups)
        {
            var UserGroups = JsonConvert.DeserializeObject<List<string>>(SelectedUserGroups);
            if (UserGroups != null)
            {
                var selectedUserGroupIds = UserGroups.Select(int.Parse).ToList();
                var existingMetaDataTypeUserGroups = _context.MetaDataTypeUserGroups
                    .Where(i => i.MetaDataTypeId == metaData.Id).ToList();

                var existingUserGroupIds = existingMetaDataTypeUserGroups.Select(ug => ug.UserGroupId).ToList();

                // Find user group IDs to remove (IDs present in existing but not in selected)
                var userGroupIdsToRemove = existingUserGroupIds.Except(selectedUserGroupIds).ToList();

                // Remove associations for user group IDs to remove
                var associationsToRemove = existingMetaDataTypeUserGroups
                    .Where(association => userGroupIdsToRemove.Contains(association.UserGroupId))
                    .ToList();
                _context.MetaDataTypeUserGroups.RemoveRange(associationsToRemove);

                // Find user group IDs to add (IDs present in selected but not in existing)
                var newUserGroupIdsToAdd = selectedUserGroupIds.Except(existingUserGroupIds).ToList();

                // Retrieve the user groups to add based on IDs
                var userGroupsToAdd = _context.UserGroups
                    .Where(group => newUserGroupIdsToAdd.Contains(group.UserGroupId))
                    .ToList();

                // Create and add new associations for user groups to add
                foreach (var userGroup in userGroupsToAdd)
                {
                    var newAssociation = new MetaDataTypeUserGroup
                    {
                        MetaDataTypeId = metaData.Id,
                        MetaDataType = _context.MetaDataTypes.Find(metaData.Id),
                        UserGroupId = userGroup.UserGroupId,
                        UserGroup = userGroup
                    };

                    _context.MetaDataTypeUserGroups.Add(newAssociation);
                }

                // Save changes to the database
                _context.SaveChanges();
            }
            var metaDataType = _context.MetaDataTypes.Include(i => i.UserGroupNames)
                .Where(i => i.Id == metaData.Id).FirstOrDefault();
            if (metaDataType != null)
            {
                metaDataType.MetaDataTypeName = metaData.MetaDataTypeName;
                metaDataType.MetaDataDataType = metaData.MetaDataDataType;
                _context.SaveChanges();
                return Json(new { success = true, message = "FileNameType MetaData Updated Successfully..." });
            }
            return Json(new { success = false, message = "Some error occurred while updating FileNameType MetaData. Please try again later." });
        }

        [HttpGet]
        public IActionResult ShowAutoFileName()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var autoFileNames = _context.FileNameTypeDocuments.Where(c => c.CompanyAdminUser.AspId == claim.Value).ToList();
                var autoFileNameList = autoFileNames.Select(i => new
                {
                    Value = i.Id,
                    Text = i.FileName,
                }).ToList();

                return Json(autoFileNameList);
            }
            return Json(new { success = "false", data = new List<object>() });
        }

        [HttpPost]
        public IActionResult CreateFileTypeDocument(FileNameTypeDocument fileNameType, string SelectedMetadata)
        {
            var metaDataList = JsonConvert.DeserializeObject<List<FileNameMetaData>>(SelectedMetadata);
            if (metaDataList != null)
            {
                if (metaDataList.Count > 0)
                {
                    // Use the selected metadata list
                    fileNameType.MetaDataTypeList = metaDataList;
                }
                else
                {
                    // If metaDataList is empty, add a default metadata entry with current datetime
                    fileNameType.MetaDataTypeList = new List<FileNameMetaData>
                     {
                       new FileNameMetaData { MetadataName = DateTime.Now.ToString() }
                     };
                }
            }
            if (ModelState.IsValid)
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    var userId = claim.Value;

                    // Check if the current user is a company admin or a company user
                    var isCompanyAdmin = User.IsInRole("Company Admin");

                    if (isCompanyAdmin)
                    {
                        // If the current user is a company admin, fetch the corresponding CompanyAdminId
                        var companyAdmin = _context.CompanyAdmin.FirstOrDefault(ca => ca.AspId == userId);
                        if (companyAdmin != null)
                        {
                            fileNameType.CompanyAdminId = companyAdmin.Id;
                            fileNameType.CompanyAdminUser = companyAdmin;
                        }
                    }
                    else
                    {
                        // If the current user is a company user, fetch the corresponding CompanyAdminId
                        var companyUser = _context.CompanyUsers.FirstOrDefault(cu => cu.UserId == userId);
                        if (companyUser != null)
                        {
                            var companyAdminId = companyUser.UserId;
                            // Use the companyAdminId as needed
                        }
                    }

                    var currentUser = _context.ApplicationUsers.FirstOrDefault(i => i.Id == userId);
                    if (currentUser != null)
                    {
                        fileNameType.FileName = $"{fileNameType.FileName}_CreatedBy_{currentUser.FullName}";

                        _context.FileNameTypeDocuments.Add(fileNameType);
                    }

                    _context.SaveChanges();

                    var auditLog = new AuditLog
                    {
                        Username = currentUser.UserName,
                        Fullname = currentUser.FullName,
                        Time = DateTime.Now.TimeOfDay, // Save only the time part
                        Date = DateTime.Now.Date, // Save only the date part
                        BrowserName = HttpContext.Request.Headers["User-Agent"].ToString(),
                        IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                        //Action = "FileName Type Created",
                        RecordType = "FileName Type Created",
                        //OldData = "N/A",
                        NewData = $"New FileName Type Created No {fileNameType.Id}",
                        //Description = $"Document created by {currentUser.FullName} Document No. {fileNameType.Id}"
                    };

                    _context.AuditLogs.Add(auditLog);
                    _context.SaveChanges();

                    return Json(new { success = true, message = "FileNameType MetaData Added Successfully..." });
                }
            }

            return Json(new { success = false, message = "Some error occurred while updating FileNameType MetaData. Please try again later." });
        }

        [HttpGet]
        public IActionResult EditFileTypeDoc(int id)
        {
            var fileTypeDoc = _context.FileNameTypeDocuments
                .Include(f => f.MetaDataTypeList)
                .FirstOrDefault(i => i.Id == id);

            if (fileTypeDoc != null)
            {
                var metaDataList = fileTypeDoc.MetaDataTypeList;
                var metaDataSerialized = metaDataList.Select(metaData => new
                {
                    id = metaData.Id,
                    metaDataName = metaData.MetadataName,
                    separator = metaData.Seperator
                }).ToList();

                var data = new
                {
                    id = fileTypeDoc.Id,
                    fileName = fileTypeDoc.FileName,
                    metaDataList = metaDataSerialized,
                };

                return Json(new { success = true, message = "File Type Document found.", data });
            }

            return Json(new { success = false, message = "File Type Document Not Found." });
        }


        [HttpPost]
        public IActionResult EditFileTypeDoc(FileNameTypeDocument fileNameType, string SelectedMetadata)
        {
            var existFileType = _context.FileNameTypeDocuments.Find(fileNameType.Id);
            try
            {
                var serializerSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                // Save a copy of the existing data in the audit log before making changes
                var previousData = JsonConvert.SerializeObject(existFileType, serializerSettings);
                var metaDataList = JsonConvert.DeserializeObject<List<FileNameMetaData>>(SelectedMetadata);
                if (metaDataList != null)
                {
                    if (metaDataList.Count > 0)
                    {
                        var existingMetaDataList = _context.FileNameMetaDatas.Where(i => i.FileNameTypeDocumentId == fileNameType.Id).ToList();

                        // Update existing metadata items and remove extra items
                        for (int i = 0; i < existingMetaDataList.Count; i++)
                        {
                            if (i < metaDataList.Count)
                            {
                                existingMetaDataList[i].MetadataName = metaDataList[i].MetadataName;
                                existingMetaDataList[i].Seperator = metaDataList[i].Seperator;
                            }
                            else
                            {
                                _context.FileNameMetaDatas.Remove(existingMetaDataList[i]);
                            }
                        }

                        // Add new metadata items if needed
                        for (int i = existingMetaDataList.Count; i < metaDataList.Count; i++)
                        {
                            existingMetaDataList.Add(metaDataList[i]);
                        }

                        // Save changes to the database
                        _context.SaveChanges();
                    }
                    else
                    {
                        // If metaDataList is empty, add a default metadata entry with current datetime
                        fileNameType.MetaDataTypeList = new List<FileNameMetaData>
                     {
                       new FileNameMetaData { MetadataName = DateTime.Now.ToString() }
                     };
                    }
                }
                if (ModelState.IsValid)
                {

                    if (existFileType != null)
                    {
                        var claimIdentity = (ClaimsIdentity)User.Identity;
                        var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                        if (claim != null)
                        {
                            var userId = claim.Value;
                            var currentUser = _context.ApplicationUsers.FirstOrDefault(i => i.Id == userId);
                            if (currentUser != null)
                            {
                                existFileType.FileName = $"{fileNameType.FileName}_UpdatedBy_{currentUser.FullName}";

                                // Save changes to the database
                                _context.SaveChanges();
                            }

                            var auditLog = new AuditLog
                            {
                                Username = currentUser.UserName,
                                Fullname = currentUser.FullName,
                                Time = DateTime.Now.TimeOfDay, // Save only the time part
                                Date = DateTime.Now.Date, // Save only the date part
                                BrowserName = HttpContext.Request.Headers["User-Agent"].ToString(),
                                IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                                //Action = "FileName Type Updated By" + " " + currentUser.FullName,
                                RecordType = "FileName Type Updated" + " " + fileNameType.FileName,
                                //OldData = previousData,
                                NewData = $"FileName Type Updated No {fileNameType.Id}",
                                //Description = $"FileName Type Updated by {currentUser.FullName} FileName Type No." +
                                // $" {fileNameType.Id} Previus FileName Type MetaData" + " " + previousData
                            };

                            _context.AuditLogs.Add(auditLog);
                            _context.SaveChanges();
                        }
                    }
                    return Json(new { success = true, message = $" File Type Document'{existFileType.FileName}'  Updated Successfully..." });
                }
                return Json(new { success = false, message = "Some error occurred while updating File Type Document. Please try again later." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }
        [HttpGet]
        public IActionResult FileTypeMetaDataIndex(string id)
        {
            // Use the id parameter as needed in your CompanyAdmin action
            ViewBag.RandomString = id;

            // If id is not provided or is empty, generate a new random code
            if (string.IsNullOrEmpty(id))
            {
                string randomCode = GenerateRandomCode();
                ViewBag.RandomString = randomCode;
                return RedirectToAction("FileTypeMetaDataIndex", new { id = randomCode });
            }

            return View();
        }

        [HttpGet]
        public IActionResult MetaDataTypeIndex(string id)
        {
            // Use the id parameter as needed in your CompanyAdmin action
            ViewBag.RandomString = id;

            // If id is not provided or is empty, generate a new random code
            if (string.IsNullOrEmpty(id))
            {
                string randomCode = GenerateRandomCode();
                ViewBag.RandomString = randomCode;
                return RedirectToAction("MetaDataTypeIndex", new { id = randomCode });
            }

            return View();
        }


        //Download Excel file of MetaData Type file...
        public IActionResult ExportExcelMetaData()
        {
            try
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    var data = _context.MetaDataTypes.Include(i => i.CompanyAdminUser)
                       .Include(i => i.UserGroupNames).ThenInclude(ug => ug.UserGroup)
                       .Where(c => c.CompanyAdminUser.AspId == claim.Value).ToList();
                    if (data != null && data.Count > 0)
                    {
                        // Select only the "GroupName" and "UserName" columns from the data
                        var filteredData = data.Select(group => new
                        {
                            MetaDataName = group.MetaDataTypeName,
                            MetaDataType = group.MetaDataDataType,
                            GroupName = group.UserGroupNames
                        }).ToList();

                        if (filteredData.Count > 0)
                        {
                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                var ws = wb.Worksheets.Add("Meta Data Type");

                                // Add table headers and format them
                                var headerRow = ws.Row(1); // Select the first row (header row)
                                headerRow.Style.Font.Bold = true; // Make the text bold
                                headerRow.Style.Font.FontSize = 12; // Set the font size

                                // Add table headers
                                ws.Cell(1, 1).Value = "Meta Data Name";
                                ws.Cell(1, 2).Value = "Meta Data Type";
                                ws.Cell(1, 3).Value = "Group Name";

                                // Add data to the table
                                for (int i = 0; i < filteredData.Count; i++)
                                {
                                    ws.Cell(i + 2, 1).Value = filteredData[i].MetaDataName;
                                    ws.Cell(i + 2, 2).Value = filteredData[i].MetaDataType;

                                    string groupNames = string.Join(", ", filteredData[i].GroupName.Select(ug => ug.UserGroup?.UserGroupName));
                                    ws.Cell(i + 2, 3).Value = groupNames;
                                }

                                using (MemoryStream stream = new MemoryStream())
                                {
                                    wb.SaveAs(stream);
                                    string filename = $"MetaDataType{DateTime.Now.ToString("ddMMyyyy")}.xlsx";
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

        //Download Excel file of File Type Meta Data file...
        public IActionResult ExportExcelFileType()
        {
            try
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    var data = _context.FileNameTypeDocuments.Include(i => i.CompanyAdminUser)
                       .Include(i => i.MetaDataTypeList)
                       .Where(c => c.CompanyAdminUser.AspId == claim.Value).ToList();
                    if (data != null && data.Count > 0)
                    {
                        var filteredData = data.Select(group => new
                        {
                            FileTypeName = group.FileName,
                            MetaData = group.MetaDataTypeList
                        }).ToList();

                        if (filteredData.Count > 0)
                        {
                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                var ws = wb.Worksheets.Add("File Type MetaData");

                                // Add table headers and format them
                                var headerRow = ws.Row(1); // Select the first row (header row)
                                headerRow.Style.Font.Bold = true; // Make the text bold
                                headerRow.Style.Font.FontSize = 12; // Set the font size

                                // Add table headers
                                ws.Cell(1, 1).Value = "File Name";
                                ws.Cell(1, 2).Value = "Meta Data";

                                // Add data to the table
                                for (int i = 0; i < filteredData.Count; i++)
                                {
                                    ws.Cell(i + 2, 1).Value = filteredData[i].FileTypeName;

                                    string groupNames = string.Join(", ", filteredData[i].MetaData.Select(ug => ug.MetadataName));
                                    ws.Cell(i + 2, 2).Value = groupNames;
                                }

                                using (MemoryStream stream = new MemoryStream())
                                {
                                    wb.SaveAs(stream);
                                    string filename = $"FileTypeMetaData{DateTime.Now.ToString("ddMMyyyy")}.xlsx";
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

        //Download Excel file of Document Meta Data file...
        public IActionResult ExportExcelDocument()
        {
            try
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    var data = _context.Documents.Include(i => i.CompanyAdminUser).Include(i => i.FileNameTypeDocument)
                       .Include(i => i.MetaDataList).Include(i => i.UserGroupNames).ThenInclude(ug => ug.UserGroup)
                       .Where(c => c.CompanyAdminUser.AspId == claim.Value).ToList();
                    if (data != null && data.Count > 0)
                    {
                        var filteredData = data.Select(group => new
                        {
                            DocumentName = group.DocTypeName,
                            Ocr = group.OCR,
                            Versioning = group.VERSIONING,
                            FileTypeName = group.FileNameTypeDocument.FileName,
                            MetaDataName = group.MetaDataList,
                            UserGroup = group.UserGroupNames
                        }).ToList();

                        if (filteredData.Count > 0)
                        {
                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                var ws = wb.Worksheets.Add("File Type MetaData");

                                // Add table headers and format them
                                var headerRow = ws.Row(1); // Select the first row (header row)
                                headerRow.Style.Font.Bold = true; // Make the text bold
                                headerRow.Style.Font.FontSize = 12; // Set the font size

                                // Add table headers
                                ws.Cell(1, 1).Value = "Document Name";
                                ws.Cell(1, 2).Value = "Ocr";
                                ws.Cell(1, 3).Value = "Versioning";
                                ws.Cell(1, 4).Value = "File Type Name";
                                ws.Cell(1, 5).Value = "Meta Data Name";
                                ws.Cell(1, 6).Value = "Group Name";

                                // Add data to the table
                                for (int i = 0; i < filteredData.Count; i++)
                                {
                                    ws.Cell(i + 2, 1).Value = filteredData[i].DocumentName;
                                    ws.Cell(i + 2, 2).Value = filteredData[i].Ocr.ToString() == "True" ? "Enabled" : "Disabled";
                                    ws.Cell(i + 2, 3).Value = filteredData[i].Versioning.ToString() == "True" ? "Enabled" : "Disabled";
                                    ws.Cell(i + 2, 4).Value = filteredData[i].FileTypeName;

                                    string metaData = string.Join(", ", filteredData[i].MetaDataName.Select(ug => ug.MetadataName));
                                    ws.Cell(i + 2, 5).Value = metaData;

                                    string groupNames = string.Join(", ", filteredData[i].UserGroup.Select(ug => ug.UserGroup.UserGroupName));
                                    ws.Cell(i + 2, 6).Value = groupNames;
                                }

                                using (MemoryStream stream = new MemoryStream())
                                {
                                    wb.SaveAs(stream);
                                    string filename = $"DocumentTypeMetaData{DateTime.Now.ToString("ddMMyyyy")}.xlsx";
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

        //Download Pdf file of Document Meta Data file...
        public IActionResult ExportPdfDocument()
        {
            try
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    var data = _context.Documents.Include(i => i.CompanyAdminUser).Include(i => i.FileNameTypeDocument)
                       .Include(i => i.MetaDataList).Include(i => i.UserGroupNames).ThenInclude(ug => ug.UserGroup)
                       .Where(c => c.CompanyAdminUser.AspId == claim.Value).ToList();
                    if (data != null && data.Count > 0)
                    {
                        var filteredData = data.Select(group => new
                        {
                            DocumentName = group.DocTypeName,
                            Ocr = group.OCR,
                            Versioning = group.VERSIONING,
                            FileTypeName = group.FileNameTypeDocument.FileName,
                            MetaDataName = group.MetaDataList,
                            UserGroup = group.UserGroupNames
                        }).ToList();

                        if (filteredData.Count > 0)
                        {
                            using (PdfDocument document = new PdfDocument())
                            {
                                PdfPage page = document.AddPage();
                                XGraphics gfx = XGraphics.FromPdfPage(page);

                                // Define fonts
                                XFont font = new XFont("Verdana", 10, XFontStyle.Regular);
                                XFont boldFont = new XFont("Arial", 12, XFontStyle.Bold);

                                // Define the starting position of the table
                                double x = 25;
                                double y = 30;

                                gfx.DrawString("Document Meta Data", boldFont, XBrushes.Black, new XPoint(x + 200, y));
                                y += 20;

                                // Calculate dynamic column widths based on available space
                                double docNameWidth = 100;
                                double ocrWidth = 50;
                                double versioningWidth = 70;
                                double FileTypeNameWidth = 100;
                                double metaDataNameWidth = 125;
                                double groupNameWidth = 125;
                                double cellHeight = 100; // Default cell height

                                // Draw table headers
                                DrawCell(gfx, "Document Name", font, x, y, docNameWidth, 40);
                                DrawCell(gfx, "Ocr", font, x + docNameWidth, y, ocrWidth, 40);
                                DrawCell(gfx, "Versioning", font, x + docNameWidth + ocrWidth, y, versioningWidth, 40);
                                DrawCell(gfx, "FileType Name", font, x + docNameWidth + ocrWidth + versioningWidth, y, FileTypeNameWidth, 40);
                                DrawCell(gfx, "MetaData Name", font, x + docNameWidth + ocrWidth + versioningWidth + FileTypeNameWidth, y, metaDataNameWidth, 40);
                                DrawCell(gfx, "Group Name", font, x + docNameWidth + ocrWidth + versioningWidth + FileTypeNameWidth + metaDataNameWidth, y, groupNameWidth, 40);

                                y += 40;

                                // Add data to the table
                                foreach (var item in filteredData)
                                {
                                    // Wrap the text within the cell based on words
                                    string documentName = item.DocumentName;
                                    List<string> lines = new List<string>();
                                    var format = new XStringFormat();

                                    string ocr = item.Ocr.ToString() == "True" ? "Enabled" : "Disabled";
                                    string versioning = item.Versioning.ToString() == "True" ? "Enabled" : "Disabled";
                                    string fileTypeName = item.FileTypeName;
                                    // MetaData Names
                                    List<string> metaDataNames = item.MetaDataName.Select(meta => meta.MetadataName).ToList();

                                    // Group Names
                                    List<string> groupNames = item.UserGroup.Select(group => group.UserGroup.UserGroupName).ToList();

                                    XRect documentNameRect = new XRect(x, y, docNameWidth, cellHeight);

                                    string[] words = documentName.Split(' ');
                                    StringBuilder line = new StringBuilder();

                                    foreach (var word in words)
                                    {
                                        double lineWidth = gfx.MeasureString(line + word, font).Width;

                                        if (lineWidth < documentNameRect.Width)
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


                                    // Split and wrap text for FileTypeName                                    
                                    XRect fileTypeNameRect = new XRect(x + docNameWidth + ocrWidth + versioningWidth, y, FileTypeNameWidth, maxRowHeight);

                                    string[] fileTypeWords = fileTypeName.Split(' ', '_', '-', ';', ',', '.');
                                    List<string> fileTypeLines = new List<string>();
                                    StringBuilder fileTypeLine = new StringBuilder();
                                    foreach (var word in fileTypeWords)
                                    {
                                        double lineWidth = gfx.MeasureString(fileTypeLine + word, font).Width;

                                        if (lineWidth < fileTypeNameRect.Width)
                                        {
                                            fileTypeLine.Append(word + " ");
                                        }
                                        else
                                        {
                                            fileTypeLines.Add(fileTypeLine.ToString());
                                            fileTypeLine.Clear();
                                            fileTypeLine.Append(word + " ");
                                        }
                                    }
                                    fileTypeLines.Add(fileTypeLine.ToString());

                                    // Calculate the height required for the wrapped text
                                    double fileTypeWrappedTextHeight = fileTypeLines.Count * font.Height;

                                    // Calculate the maximum height needed for the current row
                                    double fileTypeMaxRowHeight = Math.Max(fileTypeWrappedTextHeight, maxRowHeight);

                                    string metaDataCombined = string.Join(", ", metaDataNames.Select(name => name.ToString()));
                                    XRect metaDataNameRect = new XRect(x + docNameWidth + ocrWidth + versioningWidth + FileTypeNameWidth, y, metaDataNameWidth, maxRowHeight);

                                    string[] metaDataWords = metaDataCombined.Split(' ');
                                    List<string> metaDataLines = new List<string>();
                                    StringBuilder metaDataline = new StringBuilder();

                                    foreach (var word in metaDataWords)
                                    {
                                        double lineWidth = gfx.MeasureString(metaDataline + word, font).Width;

                                        if (lineWidth < metaDataNameRect.Width)
                                        {
                                            metaDataline.Append(word + " ");
                                        }
                                        else
                                        {
                                            metaDataLines.Add(metaDataline.ToString());
                                            metaDataline.Clear();
                                            metaDataline.Append(word + " ");
                                        }
                                    }
                                    metaDataLines.Add(metaDataline.ToString());

                                    // Calculate the height required for the wrapped text
                                    double metaDataWrappedTextHeight = metaDataLines.Count * font.Height;

                                    // Calculate the maximum height needed for the current row
                                    double metaDataMaxRowHeight = Math.Max(metaDataWrappedTextHeight, maxRowHeight);

                                    string userGroupName = string.Join(", ", groupNames.Select(name => name.ToString()));
                                    XRect userGroupNameRect = new XRect(x + docNameWidth + ocrWidth + versioningWidth + FileTypeNameWidth + metaDataNameWidth, y, groupNameWidth, maxRowHeight);
                                    string[] groupNameWords = userGroupName.Split(' ');
                                    List<string> groupNameLines = new List<string>();
                                    StringBuilder groupNameline = new StringBuilder();

                                    foreach (var word in groupNameWords)
                                    {
                                        double lineWidth = gfx.MeasureString(groupNameline + word, font).Width;

                                        if (lineWidth < userGroupNameRect.Width)
                                        {
                                            groupNameline.Append(word + " ");
                                        }
                                        else
                                        {
                                            groupNameLines.Add(groupNameline.ToString());
                                            groupNameline.Clear();
                                            groupNameline.Append(word + " ");
                                        }
                                    }
                                    groupNameLines.Add(groupNameline.ToString());

                                    // Calculate the height required for the wrapped text
                                    double groupNameWrappedTextHeight = groupNameLines.Count * font.Height;

                                    // Calculate the maximum height needed for the current row
                                    double groupNameMaxRowHeight = Math.Max(groupNameWrappedTextHeight, maxRowHeight);

                                    // Calculate the height required for each cell's content
                                    double documentNameContentHeight = wrappedTextHeight;
                                    double ocrContentHeight = maxRowHeight;
                                    double versioningContentHeight = maxRowHeight;
                                    double fileTypeNameContentHeight = fileTypeMaxRowHeight;
                                    double metaDataNameContentHeight = metaDataMaxRowHeight;
                                    double groupNameContentHeight = groupNameMaxRowHeight;

                                    // Calculate the maximum height needed for the current row
                                    double maxRowContentHeight = Math.Max(
                                        documentNameContentHeight,
                                        Math.Max(ocrContentHeight,
                                            Math.Max(versioningContentHeight,
                                                Math.Max(fileTypeNameContentHeight,
                                                    Math.Max(metaDataNameContentHeight, groupNameContentHeight)
                                                )
                                            )
                                        )
                                    );

                                    // Set the row height based on the maximum content height
                                    double rowHeight = maxRowContentHeight;

                                    // Draw the wrapped text in the user name cell, adjusting the height to match the group name cell
                                    for (int i = 0; i < lines.Count; i++)
                                    {
                                        double lineY = y + i * (maxRowHeight / lines.Count);
                                        DrawCell(gfx, lines[i], font, x, lineY, docNameWidth, rowHeight);
                                    }

                                    DrawCell(gfx, ocr, font, x + docNameWidth, y, ocrWidth, rowHeight);
                                    DrawCell(gfx, versioning, font, x + docNameWidth + ocrWidth, y, versioningWidth, rowHeight);

                                    // Draw the wrapped text in the FileType Name cell
                                    for (int i = 0; i < fileTypeLines.Count; i++)
                                    {
                                        double lineY = y + i * (fileTypeMaxRowHeight / fileTypeLines.Count);
                                        DrawCell(gfx, fileTypeLines[i], font, x + docNameWidth + ocrWidth + versioningWidth, lineY, FileTypeNameWidth, rowHeight / fileTypeLines.Count);
                                    }

                                    // Draw the wrapped text in the MetaData Name cell
                                    for (int i = 0; i < metaDataLines.Count; i++)
                                    {
                                        double lineY = y + i * (metaDataMaxRowHeight / metaDataLines.Count);
                                        DrawCell(gfx, metaDataLines[i], font, x + docNameWidth + ocrWidth + versioningWidth + FileTypeNameWidth, lineY, metaDataNameWidth, rowHeight / metaDataLines.Count);
                                    }

                                    // Draw the wrapped text in the Group Name cell
                                    for (int i = 0; i < groupNameLines.Count; i++)
                                    {
                                        double lineY = y + i * (groupNameMaxRowHeight / groupNameLines.Count);
                                        DrawCell(gfx, groupNameLines[i], font, x + docNameWidth + ocrWidth + versioningWidth + FileTypeNameWidth + metaDataNameWidth, lineY, groupNameWidth, rowHeight / groupNameLines.Count);
                                    }

                                    y += rowHeight;
                                }

                                using (MemoryStream stream = new MemoryStream())
                                {
                                    document.Save(stream, false);
                                    string filename = $"DocumentType_{DateTime.Now.ToString("ddMMyyyy")}.pdf";
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

        //Download Pdf file of FileType Meta Data file...
        public IActionResult ExportPdfFileType()
        {
            try
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    var data = _context.FileNameTypeDocuments.Include(i => i.CompanyAdminUser).Include(i => i.MetaDataTypeList)
                       .Where(c => c.CompanyAdminUser.AspId == claim.Value).ToList();
                    if (data != null && data.Count > 0)
                    {
                        var filteredData = data.Select(group => new
                        {
                            Name = group.FileName,
                            MetaDataName = group.MetaDataTypeList.Select(i => i.MetadataName).ToList(),
                        }).ToList();

                        if (filteredData.Count > 0)
                        {
                            using (PdfDocument document = new PdfDocument())
                            {
                                PdfPage page = document.AddPage();
                                XGraphics gfx = XGraphics.FromPdfPage(page);

                                // Define fonts
                                XFont font = new XFont("Verdana", 10, XFontStyle.Regular);
                                XFont headerFont = new XFont("Arial", 10, XFontStyle.Bold);
                                XFont boldFont = new XFont("Arial", 12, XFontStyle.Bold);

                                // Define the starting position of the table
                                double x = 25;
                                double y = 30;

                                gfx.DrawString("FileType Meta Data", boldFont, XBrushes.Black, new XPoint(x + 200, y));
                                y += 20;

                                // Calculate dynamic column widths based on available space
                                double FileNameWidth = 280;
                                double metaDataNameWidth = 280;
                                double cellHeight = 40; // Default cell height

                                // Draw table headers
                                DrawCell(gfx, "File Type Name", headerFont, x, y, FileNameWidth, cellHeight);
                                DrawCell(gfx, "MetaData Name", headerFont, x + FileNameWidth, y, metaDataNameWidth, cellHeight);

                                y += cellHeight;

                                // Add data to the table
                                foreach (var item in filteredData)
                                {
                                    // Wrap the text within the cell based on words
                                    string fileTypeName = item.Name;
                                    List<string> lines = new List<string>();
                                    var format = new XStringFormat();

                                    XRect fileTypeNameRect = new XRect(x, y, FileNameWidth, cellHeight);

                                    string[] words = fileTypeName.Split('_', '-', ':', ' ');
                                    StringBuilder line = new StringBuilder();

                                    foreach (var word in words)
                                    {
                                        double lineWidth = gfx.MeasureString(line + word, font).Width;

                                        if (lineWidth < fileTypeNameRect.Width)
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

                                    // MetaData Names
                                    List<string> metaDataNames = item.MetaDataName.Select(meta => meta).ToList();
                                    string metaDataName = string.Join(", ", metaDataNames.Select(name => name.ToString()));
                                    XRect metaDataNameRect = new XRect(x, y, metaDataNameWidth, cellHeight);

                                    string[] metaDataWords = metaDataName.Split(' ');
                                    List<string> metaDataNameLines = new List<string>();
                                    StringBuilder metaDataline = new StringBuilder();

                                    foreach (var word in metaDataWords)
                                    {
                                        double lineWidth = gfx.MeasureString(metaDataline + word, font).Width;

                                        if (lineWidth < metaDataNameRect.Width)
                                        {
                                            metaDataline.Append(word + " ");
                                        }
                                        else
                                        {
                                            metaDataNameLines.Add(metaDataline.ToString());
                                            metaDataline.Clear();
                                            metaDataline.Append(word + " ");
                                        }
                                    }
                                    metaDataNameLines.Add(metaDataline.ToString());

                                    // Calculate the height required for the wrapped text
                                    double metaDataNameWrappedTextHeight = metaDataNameLines.Count * font.Height;

                                    // Calculate the maximum height needed for the current row
                                    double metaDataMaxRowHeight = Math.Max(metaDataNameWrappedTextHeight, maxRowHeight);

                                    // Calculate the maximum height needed for the current row
                                    double maxRowContentHeight = Math.Max(wrappedTextHeight,
                                        Math.Max(maxRowHeight, metaDataMaxRowHeight));

                                    // Set the row height based on the maximum content height
                                    double rowHeight = maxRowContentHeight;

                                    // Draw the wrapped text in the FileType name cell, adjusting the height to match the FileType name cell
                                    for (int i = 0; i < lines.Count; i++)
                                    {
                                        double lineY = y + i * (maxRowHeight / lines.Count);
                                        DrawCell(gfx, lines[i], font, x, lineY, FileNameWidth, rowHeight / lines.Count);
                                    }

                                    // Draw the wrapped text in the MetaData Name cell
                                    for (int i = 0; i < metaDataNameLines.Count; i++)
                                    {
                                        double lineY = y + i * (metaDataMaxRowHeight / metaDataNameLines.Count);
                                        DrawCell(gfx, metaDataNameLines[i], font, x + FileNameWidth, lineY, metaDataNameWidth, rowHeight / metaDataNameLines.Count);
                                    }

                                    y += rowHeight;
                                }

                                using (MemoryStream stream = new MemoryStream())
                                {
                                    document.Save(stream, false);
                                    string filename = $"FileTypeName_{DateTime.Now.ToString("ddMMyyyy")}.pdf";
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

        //Download Pdf file of Meta Data Type file...
        public IActionResult ExportPdfMetaDataType()
        {
            try
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim != null)
                {
                    var data = _context.MetaDataTypes.Include(i => i.CompanyAdminUser)
                        .Include(i => i.UserGroupNames).ThenInclude(ug => ug.UserGroup)
                       .Where(c => c.CompanyAdminUser.AspId == claim.Value).ToList();
                    if (data != null && data.Count > 0)
                    {
                        var filteredData = data.Select(group => new
                        {
                            MetaDataName = group.MetaDataTypeName,
                            MetaDataType = group.MetaDataDataType,
                            GroupName = group.UserGroupNames.Select(i => i.UserGroup.UserGroupName).ToList(),
                        }).ToList();

                        if (filteredData.Count > 0)
                        {
                            using (PdfDocument document = new PdfDocument())
                            {
                                PdfPage page = document.AddPage();
                                XGraphics gfx = XGraphics.FromPdfPage(page);

                                // Define fonts
                                XFont font = new XFont("Verdana", 10, XFontStyle.Regular);
                                XFont headerFont = new XFont("Arial", 10, XFontStyle.Bold);
                                XFont boldFont = new XFont("Arial", 12, XFontStyle.Bold);

                                // Define the starting position of the table
                                double x = 25;
                                double y = 30;

                                gfx.DrawString("Meta Data Type", boldFont, XBrushes.Black, new XPoint(x + 200, y));
                                y += 20;

                                // Calculate dynamic column widths based on available space
                                double metaDataNameWidth = 100;
                                double metaDataTypeWidth = 100;
                                double groupNameWidth = 360;
                                double cellHeight = 40; // Default cell height

                                // Draw table headers
                                DrawCell(gfx, "MetaData Name", headerFont, x, y, metaDataNameWidth, cellHeight);
                                DrawCell(gfx, "MetaData Type", headerFont, x + metaDataNameWidth, y, metaDataTypeWidth, cellHeight);
                                DrawCell(gfx, "Group Name", headerFont, x + metaDataNameWidth + metaDataTypeWidth, y, groupNameWidth, cellHeight);

                                y += cellHeight;

                                // Add data to the table
                                foreach (var item in filteredData)
                                {
                                    // Wrap the text within the cell based on words
                                    string metaDataName = item.MetaDataName;
                                    string metaDataType = item.MetaDataType;

                                    List<string> metaDatalines = new List<string>();
                                    var format = new XStringFormat();

                                    XRect metaDataNameRect = new XRect(x, y, metaDataNameWidth, cellHeight);

                                    string[] words = metaDataName.Split(' ');
                                    StringBuilder line = new StringBuilder();

                                    foreach (var word in words)
                                    {
                                        double lineWidth = gfx.MeasureString(line + word, font).Width;

                                        if (lineWidth < metaDataNameRect.Width)
                                        {
                                            line.Append(word + " ");
                                        }
                                        else
                                        {
                                            metaDatalines.Add(line.ToString());
                                            line.Clear();
                                            line.Append(word + " ");
                                        }
                                    }
                                    metaDatalines.Add(line.ToString());

                                    // Calculate the height required for the wrapped text
                                    double wrappedTextHeight = metaDatalines.Count * font.Height;

                                    // Calculate the maximum height needed for the current row
                                    double maxRowHeight = Math.Max(wrappedTextHeight, cellHeight);

                                    // MetaData Names
                                    List<string> groupNames = item.GroupName.Select(meta => meta).ToList();
                                    string userGroupName = string.Join(", ", groupNames.Select(name => name.ToString()));
                                    XRect groupNameRect = new XRect(x, y, groupNameWidth, cellHeight);

                                    string[] groupNameWords = userGroupName.Split(' ');
                                    List<string> groupNameLines = new List<string>();
                                    StringBuilder groupNameline = new StringBuilder();

                                    foreach (var word in groupNameWords)
                                    {
                                        double lineWidth = gfx.MeasureString(groupNameline + word, font).Width;

                                        if (lineWidth < metaDataNameRect.Width)
                                        {
                                            groupNameline.Append(word + " ");
                                        }
                                        else
                                        {
                                            groupNameLines.Add(groupNameline.ToString());
                                            groupNameline.Clear();
                                            groupNameline.Append(word + " ");
                                        }
                                    }
                                    groupNameLines.Add(groupNameline.ToString());

                                    // Calculate the height required for the wrapped text
                                    double groupNameWrappedTextHeight = groupNameLines.Count * font.Height;

                                    // Calculate the maximum height needed for the current row
                                    double groupMaxRowHeight = Math.Max(groupNameWrappedTextHeight, maxRowHeight);

                                    // Calculate the maximum height needed for the current row
                                    double maxRowContentHeight = Math.Max(wrappedTextHeight,
                                        Math.Max(maxRowHeight, groupMaxRowHeight));

                                    // Set the row height based on the maximum content height
                                    double rowHeight = maxRowContentHeight;

                                    // Draw the wrapped text in the FileType name cell, adjusting the height to match the FileType name cell
                                    for (int i = 0; i < metaDatalines.Count; i++)
                                    {
                                        double lineY = y + i * (maxRowHeight / metaDatalines.Count);
                                        DrawCell(gfx, metaDatalines[i], font, x, lineY, metaDataNameWidth, rowHeight / metaDatalines.Count);
                                    }

                                    DrawCell(gfx, metaDataType, font, x + metaDataNameWidth, y, metaDataTypeWidth, rowHeight / metaDatalines.Count);

                                    // Draw the wrapped text in the MetaData Name cell
                                    for (int i = 0; i < groupNameLines.Count; i++)
                                    {
                                        double lineY = y + i * (groupMaxRowHeight / groupNameLines.Count);
                                        DrawCell(gfx, groupNameLines[i], font, x + metaDataNameWidth + metaDataTypeWidth, lineY, groupNameWidth, rowHeight / groupNameLines.Count);
                                    }

                                    y += rowHeight;
                                }

                                using (MemoryStream stream = new MemoryStream())
                                {
                                    document.Save(stream, false);
                                    string filename = $"FileTypeName_{DateTime.Now.ToString("ddMMyyyy")}.pdf";
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
                var userId = claim.Value;
                var isCompanyAdmin = User.IsInRole("Company Admin");
                var user = _context.CompanyUsers.FirstOrDefault(cu => cu.AspId == userId);
                var companyAdmin = isCompanyAdmin
                    ? _context.CompanyAdmin.FirstOrDefault(ca => ca.AspId == userId)
                    : (user != null ? _context.CompanyAdmin.Find(user.UserId) : null);

                if (companyAdmin != null)
                {
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
                                    CompanyAdminId = companyAdmin.Id,
                                    CompanyAdminUser = companyAdmin
                                };
                                if (metaData != null)
                                {
                                    metaDataList.Add(metaData);
                                }
                            }

                            // Save all metadata to the database
                            _context.MetaDataTypes.AddRange(metaDataList);
                            await _context.SaveChangesAsync();

                            var auditLog = new AuditLog
                            {
                                Username = companyAdmin.AdminEmail,
                                Fullname = companyAdmin.Name,
                                Time = DateTime.Now.TimeOfDay, // Save only the time part
                                Date = DateTime.Now.Date, // Save only the date part
                                BrowserName = HttpContext.Request.Headers["User-Agent"].ToString(),
                                IPAddress = HttpContext.Connection.RemoteIpAddress.ToString(),
                                //Action = "MetaData Type Imported",
                                RecordType = "MetaData Type Imported",
                                //OldData = "N/A",
                                NewData = $"MetaData Type Imported No {metaDataList.Count}",
                                //Description = $"MetaData Type Imported by {companyAdmin.Name} & Imported No. {metaDataList.Count}"
                            };

                            _context.AuditLogs.Add(auditLog);
                            _context.SaveChanges();
                        }
                    }
                }

                return Json(new { status = true });
            }
            catch (Exception ex)
            {

                throw;
            }
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
                var currentUser = _context.ApplicationUsers.Find(claim.Value);
                if (currentUser != null)
                {
                    var user = _context.Documents.Include(i => i.FileNameTypeDocument)
                        .Where(i => i.CompanyAdminUser.AspId == currentUser.Id)
                    .Select(d => new
                    {
                        id = d.Id,
                        docTypeName = d.DocTypeName,
                        OCR = (bool)d.OCR,
                        Versioning = (bool)d.VERSIONING,
                        AutoFileName = d.FileNameTypeDocument.FileName
                    }).ToList();

                    return Json(new { data = user, status = true });
                }
            }
            return Ok();
        }


        [HttpGet]
        public IActionResult GetAllDocTypeMetadata()
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
                    var metaDataFilterList = _context.MetaDataTypes
                        .Where(i => i.CompanyAdminId == companyAdmin.Id)
                        .Select(i => new
                        {
                            id = i.Id,
                            MetadataName = i.MetaDataTypeName
                        })
                        .ToList();

                    var metaDataList = _context.MetaDataTypes
                        .Where(metaData => metaData.CompanyAdminId == null)
                        .Select(i => new
                        {
                            id = i.Id,
                            MetadataName = i.MetaDataTypeName,
                        })
                        .ToList();

                    var containerData = _context.ContainerTypes
                        .Where(i => i.CompanyAdminUser.AspId == userId)
                        .Select(i => new
                        {
                            id = i.Id,
                            containerName = i.ContainerName,
                            //  containerDataType = i.ContainerDataType
                        })
                        .ToList();

                    var selectedContainerIds = containerData.Select(c => c.id).ToList();

                    var values = _context.Values
                        .Where(i => selectedContainerIds.Contains(i.ContainerId))
                        .Select(i => new
                        {
                            id = i.Id,
                            valueId = i.ValueId,
                            name = i.Name,
                            containerId = i.ContainerId,
                        })
                        .ToList();

                    var combinedMetaDataList = isCompanyAdmin
                        ? metaDataFilterList.Concat(metaDataList).ToList()
                        : metaDataFilterList.Concat(metaDataList).ToList();

                    return Json(new { success = true, metaData = combinedMetaDataList, containerData = containerData, values = values });
                }
            }

            return Json(new { success = false });
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

        [HttpGet]
        public IActionResult GetAllFileNameTypeDocument()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var fileTypeName = _context.FileNameTypeDocuments.Where(i => i.CompanyAdminUser.AspId == claim.Value).Include(x => x.MetaDataTypeList).ToList();
                var fileTypeNameWithMetadata = fileTypeName.Select(f => new
                {
                    Ids = f.Id,
                    FileNames = f.FileName,
                    MetaDataTypeList = string.Join(", ", f.MetaDataTypeList.Select(m => m.MetadataName))
                });

                return Json(new { success = true, data = fileTypeNameWithMetadata });
            }
            return Json(new { success = false });
        }
        [HttpGet]

        public IActionResult GetAllFile(int id)
        {

            var user = _context.FileNameTypeDocuments.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                var userDto = new
                {
                    id = user.Id,
                    fileNames = user.FileName,
                    metadatalist = string.Join(", ", user.MetaDataTypeList.Select(m => m.MetadataName))
                };

                return Json(userDto);
            }

            return Json(null);
        }

        [HttpGet]
        public IActionResult GetAllMetaDataType()
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
                    var metaDataFilterList = _context.MetaDataTypes
                        .Where(i => i.CompanyAdminId == companyAdmin.Id)
                        .OrderByDescending(i => i.Id)
                        .Select(i => new
                        {
                            id = i.Id,
                            MetadataName = i.MetaDataTypeName,
                            metaDatatype = i.MetaDataDataType,
                            userGroup = i.UserGroupNames.Select(i => new
                            {
                                groupName = i.UserGroup.UserGroupName
                            }).ToList()
                        }).ToList();

                    return Json(new { success = true, data = metaDataFilterList });
                }
            }
            return Json(new { success = false, message = "MetaData not found...." });
        }

        //[HttpDelete]
        //public IActionResult DeleteFileTypeDoc(int id)
        //{
        //    var fileTypeDoc = _context.FileNameTypeDocuments
        //        .Where(i => i.Id == id)
        //        .Include(i => i.MetaDataTypeList)
        //        .FirstOrDefault();

        //    if (fileTypeDoc != null)
        //    {
        //        try
        //        {
        //            var fileNames = fileTypeDoc.FileName; // Assuming FileNames is a property in your model

        //            _context.FileNameTypeDocuments.Remove(fileTypeDoc);
        //            _context.SaveChanges();

        //            return Json(new { success = true, message = $"FileType '{fileNames}' Deleted Successfully...",  });
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex.Message);
        //            return Json(new { success = false, message = "An error occurred during the request." });
        //        }
        //    }

        //    return Json(new { success = false, message = "FileType Document Data not found..." });
        //}


        [HttpDelete]
        public IActionResult DeleteFileTypeDoc(int id)
        {
            var fileTypeDoc = _context.FileNameTypeDocuments
                .Where(i => i.Id == id)
                .Include(i => i.MetaDataTypeList)
                .FirstOrDefault();

            if (fileTypeDoc != null)
            {
                try
                {
                    var fileNames = fileTypeDoc.FileName; // Assuming FileName is a property in your model

                    // Check if the filename is used in any documents
                    var isFileNameUsed = _context.Documents.Any(d => d.FileNameTypeDocument.Id == fileTypeDoc.Id);

                    if (isFileNameUsed)
                    {
                        return Json(new { success = false, message = $"Cannot delete  because it is used in another document." });
                    }

                    // Proceed with deletion if the filename is not used in any documents
                    _context.FileNameTypeDocuments.Remove(fileTypeDoc);
                    _context.SaveChanges();

                    return Json(new { success = true, message = $"FileType '{fileNames}' Deleted Successfully..." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.ToString()}"); // Log the exception details
                    return Json(new { success = false, message = "An error occurred during the request." });
                }
            }

            return Json(new { success = false, message = "FileType Document Data not found..." });
        }


        [HttpDelete]
        public IActionResult DeleteMetaData(int id)
        {
            try
            {
                Console.WriteLine($"DeleteMetaData action reached with id: {id}");

                var metaDataType = _context.MetaDataTypes.Find(id);

                if (metaDataType == null)
                {
                    Console.WriteLine($"FileType MetaData not found with id: {id}");
                    return Json(new { success = false, message = "FileType MetaData not found..." });
                }

                _context.MetaDataTypes.Remove(metaDataType);
                _context.SaveChanges();

                Console.WriteLine($"FileType MetaData Deleted Successfully with id: {id}");

                return Json(new { success = true, message = "FileType MetaData Deleted Successfully..." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting metadata: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while deleting metadata." });
            }
        }

        //[HttpDelete]
        //public IActionResult DeleteMetaData(int id)
        //{
        //    try
        //    {
        //        Console.WriteLine($"DeleteMetaData action reached with id: {id}");

        //        var existingMetaDataUserGroups = _context.MetaDataTypeUserGroups
        //            .Include(c => c.MetaDataType)
        //            .Where(i => i.MetaDataTypeId == id)
        //            .ToList();

        //        if (existingMetaDataUserGroups.Any())
        //        {
        //            // Remove the associated MetaDataTypeUserGroups
        //            _context.MetaDataTypeUserGroups.RemoveRange(existingMetaDataUserGroups);

        //            var metaDataType = _context.MetaDataTypes.Find(id);
        //            if (metaDataType != null)
        //            {
        //                _context.MetaDataTypes.Remove(metaDataType);
        //            }

        //            _context.SaveChanges();

        //            return Json(new { success = true, message = "FileType MetaData Deleted Successfully..." });
        //        }

        //        return Json(new { success = false, message = "FileType MetaData not found..." });
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error deleting metadata: {ex.Message}");
        //        return Json(new { success = false, message = "An error occurred while deleting metadata." });
        //    }
        //}


        [HttpDelete]
        public IActionResult DeleteDocument(int id)
        {
            var documentTypeMetaData = _context.Documents.Include(i => i.UserGroupNames)
                .Include(i => i.MetaDataList).Where(i => i.Id == id).FirstOrDefault();

            if (documentTypeMetaData != null)
            {
                // Capture the document type name before deletion
                var documentTypeName = documentTypeMetaData.DocTypeName;

                var metaData = _context.MetaData.Where(i => i.DocumentsId == documentTypeMetaData.Id).ToList();
                if (metaData.Any())
                {
                    _context.MetaData.RemoveRange(metaData);
                    _context.SaveChanges();
                }

                var documentUserGroups = _context.DocumentsUserGroups.Where(i => i.DocumentsId == documentTypeMetaData.Id).ToList();
                if (documentUserGroups.Any())
                {
                    _context.DocumentsUserGroups.RemoveRange(documentUserGroups);
                    _context.SaveChanges();
                }

                _context.Documents.Remove(documentTypeMetaData);
                _context.SaveChanges();

                // Use the captured document type name in the success message
                return Json(new { success = true, message = $"Document Type '{documentTypeName}' Deleted Successfully..." });
            }

            return Json(new { success = false, message = "Document Type MetaData not found..." });
        }
        #endregion
    }
}