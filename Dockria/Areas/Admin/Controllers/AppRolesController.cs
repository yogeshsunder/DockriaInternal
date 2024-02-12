using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AppRolesController : Controller
    {
		private readonly RoleManager<IdentityRole> _roleManager;

		public AppRolesController(RoleManager<IdentityRole> roleManager)
		{
			_roleManager = roleManager;
		}

		// List All the Role Created by Users
		public IActionResult Index()
		{
			var roles = _roleManager.Roles;
			return View(roles);
		}

		[HttpGet]

		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]

		public async Task<IActionResult> Create(IdentityRole model)
		{
			//avoid Duplicate role
			if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();
			}
			return RedirectToAction("Index");
		}
	}
}

