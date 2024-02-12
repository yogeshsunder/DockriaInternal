using AutoMapper;
using Dockria.Data;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Profile = Domain.Model.Profile;

namespace WebApp.Areas.ProfileManagement.Controllers
{
    [Area("ProfileManagement")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Profile()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var userId = claim.Value;
                var existUser = await _context.Profiles.FirstOrDefaultAsync(c => c.AspId == userId);

                if (existUser != null)
                {
                    return View(existUser);
                }
            }
            Profile profile = new Profile();
            return View(profile);
        }

        public IActionResult GetProfileImage()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var userId = claim.Value;
                var profile = _context.Profiles.FirstOrDefault(p => p.AspId == userId);
                if (profile != null && profile.ImageData != null)
                {
                    return File(profile.ImageData, "image/jpeg");
                }
            }

            // Return a default image if no profile image is available or if the user is not authenticated
            var defaultImageFilePath = "~/adminAssets/assets/images/users/male/15.jpg";
            return File(defaultImageFilePath, "image/jpeg");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(Profile profile, IFormFile profileImage)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var userId = claim.Value;
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    try
                    {
                        var existingProfile = await _context.Profiles.FirstOrDefaultAsync(c => c.AspId == user.Id);
                        if (existingProfile != null)
                        {
                            // Update the existing profile with the new data
                            existingProfile.FirstName = profile.FirstName;
                            existingProfile.LastName = profile.LastName;
                            existingProfile.NickName = profile.NickName;
                            existingProfile.About = profile.About;
                            existingProfile.Address = profile.Address;
                            existingProfile.Designation = profile.Designation;
                            existingProfile.Email = profile.Email;
                            existingProfile.Mobile = profile.Mobile;
                            existingProfile.TwitterAct = profile.TwitterAct;
                            existingProfile.TwitterAct = profile.TwitterAct;
                            existingProfile.FacebookAct = profile.FacebookAct;
                            existingProfile.GooglePlusAct = profile.GooglePlusAct;
                            existingProfile.LinkedAct = profile.LinkedAct;

                            // Handle the image upload
                            if (profileImage != null && profileImage.Length > 0)
                            {
                                using (var memoryStream = new MemoryStream())
                                {
                                    await profileImage.CopyToAsync(memoryStream);
                                    existingProfile.ImageData = memoryStream.ToArray();
                                }
                                existingProfile.ImgUrl = "/path/to/save/image";
                            }

                            _context.Profiles.Update(existingProfile);
                            await _context.SaveChangesAsync();

                            return Json(new { success = true, message = "Profile updated successfully" });
                        }
                        profile.AspId = user.Id;
                        profile.ApplicationUser = user;

                        // Handle the image upload
                        if (profileImage != null && profileImage.Length > 0)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await profileImage.CopyToAsync(memoryStream);
                                profile.ImageData = memoryStream.ToArray();
                            }
                        }

                        _context.Profiles.Add(profile);
                        await _context.SaveChangesAsync();

                        return Json(new { success = true, message = "Profile updated successfully" });
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception and return an error message
                        return Json(new { success = false, message = "An error occurred while updating the profile." + ex.Message });
                    }
                }
            }
            return View(profile);
        }

        [HttpGet]
        public async Task<IActionResult> CampanyLogo()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var companyAdmin = await _context.CompanyAdmin.FirstOrDefaultAsync(i => i.AspId == claim.Value);
                if (companyAdmin != null)
                {
                    var companyLogo = await _context.CompanyLogos.FirstOrDefaultAsync(i => i.CompanyAdminId == companyAdmin.Id);
                    if (companyLogo != null)
                    {
                        // If the logo exists, display it
                        return View(companyLogo);
                    }
                }
            }

            // If the logo does not exist or the user is not authenticated, create a new CompanyLogo instance and return it to the view
            var companyLogos = new CompanyLogo();
            return View(companyLogos);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CampanyLogo(CompanyLogo companyLogo, IFormFile logoImage, IFormFile logoIcon)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var companyAdmin = await _context.CompanyAdmin.FirstOrDefaultAsync(i => i.AspId == claim.Value);
                if (companyAdmin != null)
                {
                    var existingLogo = await _context.CompanyLogos.FirstOrDefaultAsync(c => c.CompanyAdminId == companyAdmin.Id);
                    if (existingLogo != null)
                    {
                        // Handle the image upload
                        if (logoImage != null && logoImage.Length > 0)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await logoImage.CopyToAsync(memoryStream);
                                existingLogo.LogoData = memoryStream.ToArray();
                            }
                        }

                        // Handle the icon upload
                        if (logoIcon != null && logoIcon.Length > 0)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await logoIcon.CopyToAsync(memoryStream);
                                existingLogo.IconData = memoryStream.ToArray();
                            }
                        }

                        _context.CompanyLogos.Update(existingLogo);
                        await _context.SaveChangesAsync();

                        return Json(new { success = true, message = "Company Logo updated successfully" });
                    }

                    // If the logo does not exist, create a new CompanyLogo instance
                    companyLogo.CompanyAdminId = companyAdmin.Id;
                    companyLogo.CompanyAdminUser = companyAdmin;

                    // Handle the image upload
                    if (logoImage != null && logoImage.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await logoImage.CopyToAsync(memoryStream);
                            companyLogo.LogoData = memoryStream.ToArray();
                        }
                    }

                    // Handle the icon upload
                    if (logoIcon != null && logoIcon.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await logoIcon.CopyToAsync(memoryStream);
                            companyLogo.IconData = memoryStream.ToArray();
                        }
                    }

                    _context.CompanyLogos.Add(companyLogo);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, message = "Company Logo updated successfully" });
                }
            }

            return View(companyLogo);
        }
        [HttpGet]
        public IActionResult GetIconImage()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var companyAdmin = _context.CompanyAdmin.FirstOrDefault(i => i.AspId == claim.Value);

                if (companyAdmin != null)
                {
                    var existLogo = _context.CompanyLogos.FirstOrDefault(i => i.CompanyAdminId == companyAdmin.Id);

                    if (existLogo != null && existLogo.IconData != null)
                    {
                        return File(existLogo.IconData, "image/jpeg");
                    }
                }
            }

            // Return a default icon if no icon is available or if the user is not authenticated
            var defaultIconFilePath = "~/assets/img/defaultIcon.png";
            return File(defaultIconFilePath, "image/png");
        }

        [HttpGet]
        public IActionResult GetLogoImage()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)
            {
                var companyAdmin = _context.CompanyAdmin.FirstOrDefault(i => i.AspId == claim.Value);
                if (companyAdmin != null)
                {
                    var existLogo = _context.CompanyLogos.FirstOrDefault(i => i.CompanyAdminId == companyAdmin.Id);
                    if (existLogo != null && existLogo.LogoData != null)
                    {
                        return File(existLogo.LogoData, "image/jpeg");
                    }
                }
            }

            // Return a default image if no profile image is available or if the user is not authenticated
            var defaultImageFilePath = "~/assets/img/logoDockria.svg";
            return File(defaultImageFilePath, "image/jpeg");
        }

        [HttpGet]
        public IActionResult DeleteLogo()
        {
            // Display a confirmation view for logo deletion
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDeleteLogo()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var companyAdmin = await _context.CompanyAdmin.FirstOrDefaultAsync(i => i.AspId == claim.Value);

                if (companyAdmin != null)
                {
                    var existingLogo = await _context.CompanyLogos.FirstOrDefaultAsync(i => i.CompanyAdminId == companyAdmin.Id);

                    if (existingLogo != null && existingLogo.LogoData != null)
                    {
                        // Remove the logo data
                        existingLogo.LogoData = null;

                        _context.CompanyLogos.Update(existingLogo);
                        await _context.SaveChangesAsync();

                        return Json(new { success = true, message = "Logo data deleted successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Logo data does not exist or is already deleted" });
                    }
                }
            }

            return Json(new { success = false, message = "Error deleting logo data" });
        }

        [HttpGet]
        public IActionResult DeleteIcon()
        {
            // Display a confirmation view for icon deletion
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDeleteIcon()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var companyAdmin = await _context.CompanyAdmin.FirstOrDefaultAsync(i => i.AspId == claim.Value);

                if (companyAdmin != null)
                {
                    var existingLogo = await _context.CompanyLogos.FirstOrDefaultAsync(i => i.CompanyAdminId == companyAdmin.Id);

                    if (existingLogo != null && existingLogo.IconData != null)
                    {
                        // Remove the icon data
                        existingLogo.IconData = null;

                        _context.CompanyLogos.Update(existingLogo);
                        await _context.SaveChangesAsync();

                        return Json(new { success = true, message = "Icon data deleted successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Icon data does not exist or is already deleted" });
                    }
                }
            }

            return Json(new { success = false, message = "Error deleting icon data" });
        }

    }
}
