using Dockria.Data;
using DocumentFormat.OpenXml.InkML;
using Domain.Model.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Middleware
{

        public class SessionTokenMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<SessionTokenMiddleware> _logger;

            public SessionTokenMiddleware(RequestDelegate next, ILogger<SessionTokenMiddleware> logger)
            {
                _next = next;
                _logger = logger;
            }
        public async Task Invoke(HttpContext context)
        {
            
            
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();

                var existingSession = await dbContext.UserSessions
                    .FirstOrDefaultAsync(us => us.UserId == userId);






                var sessionToken = context.Session.GetString("SessionToken");

                if (existingSession != null && sessionToken != existingSession.SessionToken)
                {
                    // Session token mismatch, log out or redirect to login
                    _logger.LogWarning("Session token mismatch. Logging out user.");
                    await context.SignOutAsync(); // Sign out the user
                    context.Response.Redirect("/Identity/Account/Login");
                    return;
                }
            }

            await _next(context);
        }

    }
}
    


















