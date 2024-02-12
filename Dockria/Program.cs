using Application.Companies.Handlers;
using Application.Companies.Queries;
using Application.Middleware;
using AutoMapper;
using Dockria.Data;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Domain.Model;
using Domain.Model.ViewModel;
using Domain.Vaildation;
using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
//using ServiceStack;
using System;
//using RadPdf;
using System.Configuration;
using System.Reflection;
using System.Security.Claims;
using WebApp;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
    options.Secure = CookieSecurePolicy.Always; // Enforce HTTPS
});

ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddTransient<IValidator<CompanyViewModel>, CompanyValidator>();
builder.Services.AddControllersWithViews()
    .AddFluentValidation();

builder.Services.AddMediatR(
    typeof(GetCompanyViewModelsQuery).Assembly,
    typeof(CreateCompanyCommand).Assembly,
    typeof(GetCompanyByIdQuery).Assembly,
    typeof(EditCompanyCommand).Assembly,
    typeof(GetCompanyByIdDeleteQuery).Assembly
);

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddAutoMapper(typeof(GetCompanyByIdQuery));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;

    // Allow access to register and forgot password pages without authentication
    options.Events = new CookieAuthenticationEvents
    {
        OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/Identity/Account/Manage")
                || context.Request.Path.StartsWithSegments("/Identity/Account/Logout"))
            {
                context.RedirectUri = null;
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Creates SuperAdmin & Roles by default when the app runs for the first time...
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    SD.CreateRoles(roleManager).Wait();  // Call the method for creating roles.
    ApplicationDbContext.SeedSuperAdminUser(userManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseMiddleware<AuditLogMiddleware>();
app.UseAuthentication();

// Middleware to redirect unauthenticated users to the login page
app.Use(async (context, next) =>
{
    // Check if the user is authenticated
    if (!context.User.Identity.IsAuthenticated
&& context.Request.Path != "/Identity/Account/Login"
&& context.Request.Path != "/Identity/Account/Register"
&& context.Request.Path != "/Identity/Account/ForgotPassword"
&& context.Request.Path != "/Identity/Account/ForgotPasswordConfirmation"
&& context.Request.Path != "/Identity/Account/ResetPassword"
&& context.Request.Path != "/Identity/Account/ResetPasswordConfirmation")
    {
        // Redirect to login page if not authenticated and not on register or forgot password pages
        context.Response.Redirect("/Identity/Account/Login");
        return;
    }

    await next();
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
