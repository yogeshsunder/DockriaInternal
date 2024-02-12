using Dockria.Data;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Xml.Linq;

namespace WebApp.Areas.CompanyAdmin.Controllers
{
    [Area("CompanyAdmin")]
    public class FolderViewController : Controller
    {
        private ApplicationDbContext _context;
        public FolderViewController(ApplicationDbContext context)
        {
            _context = context;
        }
        private string GenerateRandomCode()
        {
            return Guid.NewGuid().ToString();
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

            return View();
        }

        [HttpPost]
        public IActionResult Create([FromBody] FolderView folderView)
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
                        folderView.CompanyAdminId = companyAdmin.Id;
                        folderView.CompanyAdminUser = companyAdmin;
                        _context.FolderViews.Add(folderView);
                        _context.SaveChanges();
                    }
                    return Json(new { success = true, message = "Folder View Document Added Successfully..." });
                }
                return View(folderView);
            }

            else { return View(); }
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            if (id == 0) return Json(new { success = false, message = "Data not found...." });
            var folderViewData = await _context.FolderViews.Include(i => i.CompanyAdminUser)
                .Include(i => i.RowsData).Where(i => i.Id == id).FirstOrDefaultAsync();
            if (folderViewData != null)
            {
                return Json(new
                {
                    success = true,
                    data = new
                    {
                        id = folderViewData.Id,
                        folderViewName = folderViewData.FolderViewName,
                        RowsData = folderViewData.RowsData.Select(row => new
                        {
                            id = row.Id,
                            selectObject = row.SelectObject,
                            objectType = row.ObjectType,
                            operators = row.Operator,
                            metaDataList = row.MetaDataList,
                            value = row.Value
                        }).ToList()
                    }
                });
            }
            else
            {
                return Json(new { success = false, message = "Data not found...." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] FolderView folderView)
        {
            if (folderView == null) return Json(new { success = false, message = "Data not found...." });
            var existFolderView = await _context.FolderViews.Include(i => i.CompanyAdminUser)
                .Include(i => i.RowsData).Where(i => i.Id == folderView.Id).FirstOrDefaultAsync();
            if (existFolderView != null)
            {
                // Track IDs of existing MetadataList items to be deleted
                var metadataIdsToDelete = new List<int>();

                // Update existing MetadataList items and identify items to delete
                foreach (var existingItem in existFolderView.RowsData)
                {
                    // Find the corresponding item in the incoming folderView.RowsData
                    var newItem = folderView.RowsData.FirstOrDefault(item => item.Id == existingItem.Id);

                    if (newItem != null)
                    {
                        // Update properties of the existing item
                        existingItem.SelectObject = newItem.SelectObject;
                        existingItem.ObjectType = newItem.ObjectType;
                        existingItem.Operator = newItem.Operator;
                        existingItem.MetaDataList = newItem.MetaDataList;
                        existingItem.Value = newItem.Value;

                        // Remove this ID from the list of IDs to delete
                        metadataIdsToDelete.Remove(existingItem.Id);
                    }
                    else
                    {
                        // The item exists in the database but not in the incoming data, mark for deletion
                        metadataIdsToDelete.Add(existingItem.Id);
                    }
                }
                // Add new MetadataList items
                foreach (var newItem in folderView.RowsData)
                {
                    if (existFolderView.RowsData.All(existingItem => existingItem.Id != newItem.Id))
                    {
                        newItem.FolderViewId = folderView.Id;
                        // This is a new MetadataList item, add it to the collection
                        existFolderView.RowsData.Add(newItem);
                    }
                }
                // Delete extra MetadataList items
                foreach (var idToDelete in metadataIdsToDelete)
                {
                    var itemToDelete = existFolderView.RowsData.FirstOrDefault(existingItem => existingItem.Id == idToDelete);

                    if (itemToDelete != null)
                    {
                        existFolderView.RowsData.Remove(itemToDelete);
                    }
                }

                // Save changes to the database
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Data updated successfully." });
            }
            return View(folderView);
        }

        [HttpGet]
        public async Task<IActionResult> GetSecondDropdownValue(string selectedValue)
        {

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var aspUser = await _context.ApplicationUsers.FirstOrDefaultAsync(i => i.Id == claim.Value);
                if (aspUser != null)
                {
                    if (string.IsNullOrEmpty(selectedValue))
                    {
                        return Json(new { message = "Data Not Found...", status = false });
                    }
                    else if (selectedValue == "Document Type")
                    {
                        var documentType = _context.Documents.Include(i => i.FileNameTypeDocument)
                        .Where(i => i.CompanyAdminUser.AspId == aspUser.Id)
                    .Select(d => new
                    {
                        id = d.Id,
                        docTypeName = d.DocTypeName
                    }).ToList();

                        return Json(new { data = documentType, status = true });
                    }
                    else if (selectedValue == "Container Type")
                    {
                        var containerType = _context.ContainerTypes.Include(i => i.CompanyAdminUser)
                            .Include(i => i.MetaDataList).Include(i => i.UserGroupNames).ThenInclude(i => i.UserGroup)
                        .Where(i => i.CompanyAdminUser.AspId == aspUser.Id)
                    .Select(d => new
                    {
                        id = d.Id,
                        containerTypeName = d.ContainerName
                    }).ToList();
                        return Json(new { data = containerType, status = true });
                    }
                }

            }
            return Json(new { message = "Data Not Found...", status = false });
        }

        [HttpGet]
        public async Task<IActionResult> GetMetaDataDropdownValue(string selectedValue, string selectedObject)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var aspUser = await _context.ApplicationUsers.FirstOrDefaultAsync(i => i.Id == claim.Value);
                if (aspUser != null)
                {
                    if (string.IsNullOrEmpty(selectedValue) || string.IsNullOrEmpty(selectedObject))
                    {
                        return Json(new { message = "Data Not Found...", status = false });
                    }
                    else if (selectedValue == "Document Type")
                    {
                        // Filter documents by selectedObject (DocumentTypeName) and select MetaDataList
                        var metaDataList = _context.Documents
                            .Include(i => i.MetaDataList)
                            .Where(i => i.CompanyAdminUser.AspId == aspUser.Id && i.DocTypeName == selectedObject)
                            .SelectMany(i => i.MetaDataList)
                            .ToList();

                        var metaDataOptions = metaDataList.Select(mt => new
                        {
                            id = mt.Id,
                            metaDataName = mt.MetadataName
                        }).ToList();

                        return Json(new { data = metaDataOptions, status = true });
                    }
                    else if (selectedValue == "Container Type")
                    {
                        var containerType = _context.ContainerTypes
                            .Include(i => i.MetaDataList)
                            .Where(i => i.CompanyAdminUser.AspId == aspUser.Id && i.ContainerName == selectedObject)
                            .SelectMany(i => i.MetaDataList)
                            .ToList();

                        var metaDataOptions = containerType.Select(mt => new
                        {
                            id = mt.Id,
                            metaDataName = mt.MetadataName
                        }).ToList();

                        return Json(new { data = containerType, status = true });
                    }
                }

            }
            return Json(new { message = "Data Not Found...", status = false });
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
                    var folderViewList = _context.FolderViews.Include(i => i.CompanyAdminUser)
                        .Include(i => i.RowsData)
                        .Where(i => i.CompanyAdminUser.AspId == aspUser.Id).Select(i => new
                        {
                            id = i.Id,
                            folderViewName = i.FolderViewName
                        }).ToList();

                    return Json(new { success = "true", data = folderViewList });
                }
            }
            return Json(new { success = false });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0) return Json(new { success = false, message = "Data not found...." });
            var folderViewData = await _context.FolderViews.Include(i => i.CompanyAdminUser)
                .Include(i => i.RowsData).Where(i => i.Id == id).FirstOrDefaultAsync();
            if (folderViewData != null)
            {
                _context.FolderViews.RemoveRange(folderViewData);
                _context.SaveChanges();

                return Json(new { success = true, message = "Data Deleted Successfully..." });
            }

            return Json(new { success = false, message = "Data not found...." });
        }
        #endregion
    }
}
