using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace WebApp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordConfirmation : PageModel
    {
        public IActionResult OnGet()
        {
            // Check if the random ID is already present in the query parameters
            if (!Request.Query.ContainsKey("id"))
            {
                // Generate a random unique ID using Guid
                string randomId = Guid.NewGuid().ToString();

                // Append the generated ID as a query parameter to the existing URL
                string confirmationUrl = Url.Page("/Account/ForgotPasswordConfirmation", null, new { id = randomId }, Request.Scheme);

                // Redirect to the new URL with the generated ID as a query parameter
                return Redirect(confirmationUrl);
            }

            // If the random ID is already present, just render the page without redirection
            return Page();
        }
    }
}
