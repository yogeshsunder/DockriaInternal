using Domain.Model.ViewModel;
using Domain.Model;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApp.Areas.Admin.Controllers
{
    public class TerminateSessionsController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // Redirect to the login page if the user is not already logged in
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
        [BindProperty]
        public TerminateSessionsViewModel ViewModel { get; set; }

        public void OnGet()
        {
            // Your logic to populate ViewModel.ActiveSessions
            ViewModel = new TerminateSessionsViewModel
            {
                ActiveSessions = GetActiveSessionsForCurrentUser()
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Your logic to terminate selected sessions
            foreach (var sessionId in ViewModel.SessionsToTerminate)
            {
                // Implement session termination logic
                // You might want to sign out the user associated with the sessionId
                // For example: await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            // Redirect the user to the login page or another appropriate location after terminating sessions
            return RedirectToPage("/Account/Login");
        }

        private List<string> GetActiveSessionsForCurrentUser()
        {
            // Your logic to retrieve active sessions for the current user
            // You might use the SessionManager or any other session management mechanism
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return SessionManager.GetActiveSessions(userId);
        }
    }
}
