using Domain.Model;
using Domain.Model.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Web.Mvc;

namespace Dockria.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Company> Companies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CompanyViewModel> CompanyViewModels { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<CompanyAdminUser> CompanyAdmin { get; set; }
        public DbSet<SmgInfo> SmgInfos { get; set; }
        public DbSet<smgViewModel> SmgViewModels { get; set; }
        public DbSet<SmgCost> SmgCosts { get; set; }
        public DbSet<PaymentCurrency> PaymentCurrencies { get; set; }
        public DbSet<PaymentInterval> PaymentIntervals { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<CompanyUsers> CompanyUsers { get; set; }
        public DbSet<UserLogin> UserLogins { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<EndUserManagement> EndUserManagements { get; set; }
        public DbSet<DocumentManagement> DocumentManagements { get; set; }
        public DbSet<RADManagement> RADManagements { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Documents> Documents { get; set; }
        public DbSet<MetaData> MetaData { get; set; }
        public DbSet<CompanyLogo> CompanyLogos { get; set; }
        public DbSet<DocumentTypeMetadata> DocumentTypeMetadata { get; set; }
        public DbSet<MetaDataType> MetaDataTypes { get; set; }
        public DbSet<FileNameTypeDocument> FileNameTypeDocuments { get; set; }
        public DbSet<FileNameMetaData> FileNameMetaDatas { get; set; }
        public DbSet<ContainerType> ContainerTypes { get; set; }
        public DbSet<ContainerMetaData> ContainerMetaDatas { get; set; }
        public DbSet<MetaDataTypeUserGroup> MetaDataTypeUserGroups { get; set; }
        public DbSet<ContainerUserGroup> ContainerUserGroups { get; set; }
        public DbSet<DocumentsUserGroup> DocumentsUserGroups { get; set; }
        public DbSet<FolderView> FolderViews { get; set; }
        public DbSet<FolderViewList> FolderViewLists { get; set; }
        public DbSet<ValueInfo> Values { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {






        //    modelBuilder.Entity<SmgCost>()
        ////.Property(e => e.Vat)
        //.HasColumnType("decimal(18, 2)"); // Adjust the precision and scale according to your requirements

            //modelBuilder.Entity<SmgCost>()
            //   // .Property(e => e.VatPercentage)
            //    .HasColumnType("decimal(18, 2)");
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SelectListGroup>().HasNoKey();

            modelBuilder.Entity<IdentityUserLogin<string>>()
                .HasKey(l => new { l.LoginProvider, l.ProviderKey });
            modelBuilder.Entity<CompanyViewModel>().HasNoKey();
            modelBuilder.Entity<Company>()
                .ToTable("Companies")
                .HasKey(c => c.CompanyId);

            modelBuilder.Entity<Company>()
                .Property(c => c.CompanyName)
                .HasColumnName("CompanyName");

            modelBuilder.Entity<Company>()
                .Property(c => c.Address)
                .HasColumnName("Address");

            modelBuilder.Entity<Company>()
                .Property(c => c.MobileNumber)
                .HasColumnName("MobileNumber");

            modelBuilder.Entity<PaymentCurrency>().HasData(
                    new PaymentCurrency
                    {
                        PaymentCurrencyId = 1,
                        PaymentCurrencyName = "Kes"
                    },
                    new PaymentCurrency
                    {
                        PaymentCurrencyId = 2,
                        PaymentCurrencyName = "USD"
                    }
                );

            modelBuilder.Entity<PaymentInterval>().HasData(
                new PaymentInterval
                {
                    PaymentIntervalId = 1,
                    PaymentIntervalName = "Monthly"
                },
                new PaymentInterval
                {
                    PaymentIntervalId = 2,
                    PaymentIntervalName = "Quaterly"
                },
                new PaymentInterval
                {
                    PaymentIntervalId = 3,
                    PaymentIntervalName = "Biannually"
                },
                new PaymentInterval
                {
                    PaymentIntervalId = 4,
                    PaymentIntervalName = "Annually"
                }
            );

            modelBuilder.Entity<PaymentType>().HasData(
                new PaymentType
                {
                    PaymentTypeId = 1,
                    PaymentTypeName = "Renewal"
                },
                new PaymentType
                {
                    PaymentTypeId = 2,
                    PaymentTypeName = "One-Time"
                }
            );

            string[] metaDataTypeNames =
            {
                "Document Create By","Document Created Date","Document Modified By","Document Modified Date","Workflow Started By User",
                "Workflow Used by Last User","Workflow Viewed by Last User","Current Date","Current Time","Current Date Time","Metadata Create By",
                "Metadata Created Date","Metadata Modified By","Metadata Modified Date","Container Create By","Container Created Date",
                "Container Modified By","Container Modified Date"
            };

            for (int i = 0; i < metaDataTypeNames.Length; i++)
            {
                try
                {
                    modelBuilder.Entity<MetaDataType>().HasData(
                        new MetaDataType
                        {
                            Id = i + 1,
                            MetaDataTypeName = metaDataTypeNames[i],
                            MetaDataDataType = "Text"
                        });
                }
                catch (Exception ex)
                {
                    // Handle the exception, print an error message, or log the details
                    Console.WriteLine($"Error while seeding MetaDataType: {ex.Message}");
                }
            }

        }

        // Method to seed the default super admin user
        public static void SeedSuperAdminUser(UserManager<ApplicationUser> userManager)
        {
            var superAdminUser = new ApplicationUser
            {
                FullName = "Super Admin",
                UserName = "super_admin@gmail.com",
                EmailAddress = "super_admin@gmail.com",
                ConfirmEmailAddress = "super_admin@gmail.com",
                PhoneNumber = "9999999999",
                Role = SD.Role_Admin
                // Set other properties as needed
            };

            // Create the user using UserManager
            var result = userManager.CreateAsync(superAdminUser, "Admin@123456").Result;

            // Check if the user was created successfully
            if (result.Succeeded)
            {
                // Assign the user to the "Admin" role
                var roleResult = userManager.AddToRoleAsync(superAdminUser, SD.Role_Admin).Result;

                // Check if the user was successfully added to the role
                if (roleResult.Succeeded)
                {
                    // Role assignment successful
                }
                else
                {
                    foreach (var error in roleResult.Errors)
                    {
                        Console.WriteLine($"Error: {error.Description}");
                    }
                }
            }
        }
    }
}