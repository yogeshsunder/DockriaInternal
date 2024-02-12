using Dockria.Data;
using Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Middleware
{
	public class AuditLogMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<AuditLogMiddleware> _logger;

		public AuditLogMiddleware(RequestDelegate next, ILogger<AuditLogMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

        public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
        {
            // Before the request

            // Log the request details
            var request = context.Request;

			// Check if it's a login request
			bool isLoginRequest = request.Path == "/Identity/Account/Login";

			// Retrieve the user information from the form data if the request method allows it
			string userName = null;            
			if (isLoginRequest && request.Method == "POST")
			{
				var form = await context.Request.ReadFormAsync();				
				userName = form["Input.Email"];                
            }
            var currentUser = dbContext.ApplicationUsers.Where(u => u.UserName == userName).FirstOrDefault();
            var log = new AuditLog
			{
				//Action = $"HTTP {request.Method} request",
				Username = userName, // Use the captured user information
                //Description="Login",
                Time = DateTime.Now.TimeOfDay, // Save only the time part
                Date = DateTime.Now.Date, // Save only the date part
                BrowserName = context.Request.Headers["User-Agent"].ToString(),
                IPAddress = context.Connection.RemoteIpAddress.ToString(),
                RecordType = isLoginRequest ? "Login" : null // Set the action type accordingly
			};

			// Save the audit log to the database if it's a login request
			if (isLoginRequest && request.Method == "POST")
			{
				// Check if the same log already exists in the database
				var existingLog = dbContext.AuditLogs.FirstOrDefault(l => l.Username == log.Username);
				if (existingLog == null)
				{
                    log.Fullname = currentUser.FullName;
                    // Save the audit log to the database
                    dbContext.AuditLogs.Add(log);
                    await dbContext.SaveChangesAsync();
                }
                else
                {
                    // Update the existing log with the new action and timestamp
                  //  existingLog.Action = log.Action;
                    existingLog.Time = log.Time;
                    existingLog.Fullname = currentUser.FullName;
                    dbContext.AuditLogs.Update(existingLog);
                    await dbContext.SaveChangesAsync();
                }
            }
            // Proceed with the request pipeline
             await _next.Invoke(context);
        
        }
    }
}
