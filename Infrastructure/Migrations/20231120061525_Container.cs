using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class Container : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConfirmEmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Time = table.Column<TimeSpan>(type: "time", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fullname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RecordType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Filename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocByte = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CompanyDomain = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignatureLimit = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentCurrencies",
                columns: table => new
                {
                    PaymentCurrencyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentCurrencyName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentCurrencies", x => x.PaymentCurrencyId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentIntervals",
                columns: table => new
                {
                    PaymentIntervalId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentIntervalName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentIntervals", x => x.PaymentIntervalId);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTypes",
                columns: table => new
                {
                    PaymentTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTypes", x => x.PaymentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "SelectListGroup",
                columns: table => new
                {
                    Disabled = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "SmgViewModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SmgName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TaxPercentage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentCurrencyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentIntervalName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmgViewModels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IPAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LoginDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyAdmin",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OfficialEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PinNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvoiceMail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReciveEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminDesignation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdminPhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileNames = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubscriptionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageSpace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateFrom = table.Column<DateTime>(type: "Date", nullable: false),
                    DateTo = table.Column<DateTime>(type: "Date", nullable: false),
                    TotalTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AspId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyAdmin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyAdmin_AspNetUsers_AspId",
                        column: x => x.AspId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompanyUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AspId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ReadPermission = table.Column<bool>(type: "bit", nullable: false),
                    WritePermission = table.Column<bool>(type: "bit", nullable: false),
                    UploadPermission = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyUsers_AspNetUsers_AspId",
                        column: x => x.AspId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypeMetadata",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MetadataName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AspId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAdminId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypeMetadata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentTypeMetadata_AspNetUsers_CompanyAdminId",
                        column: x => x.CompanyAdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NickName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    About = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImgUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    TwitterAct = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FacebookAct = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GooglePlusAct = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkedAct = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AspId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profiles_AspNetUsers_AspId",
                        column: x => x.AspId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Designation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmgInfos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SmgName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxPercentage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentIntervalId = table.Column<int>(type: "int", nullable: false),
                    PaymentCurrencyId = table.Column<int>(type: "int", nullable: false),
                    PaymentTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmgInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmgInfos_PaymentCurrencies_PaymentCurrencyId",
                        column: x => x.PaymentCurrencyId,
                        principalTable: "PaymentCurrencies",
                        principalColumn: "PaymentCurrencyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmgInfos_PaymentIntervals_PaymentIntervalId",
                        column: x => x.PaymentIntervalId,
                        principalTable: "PaymentIntervals",
                        principalColumn: "PaymentIntervalId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SmgInfos_PaymentTypes_PaymentTypeId",
                        column: x => x.PaymentTypeId,
                        principalTable: "PaymentTypes",
                        principalColumn: "PaymentTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SmgCosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Vat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SmgViewModelId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmgCosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SmgCosts_SmgViewModels_SmgViewModelId",
                        column: x => x.SmgViewModelId,
                        principalTable: "SmgViewModels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompanyLogos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogoData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CompanyAdminId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyLogos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyLogos_CompanyAdmin_CompanyAdminId",
                        column: x => x.CompanyAdminId,
                        principalTable: "CompanyAdmin",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContainerTypes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContainerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyAdminId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContainerTypes_CompanyAdmin_CompanyAdminId",
                        column: x => x.CompanyAdminId,
                        principalTable: "CompanyAdmin",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FileNameTypeDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyAdminId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileNameTypeDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileNameTypeDocuments_CompanyAdmin_CompanyAdminId",
                        column: x => x.CompanyAdminId,
                        principalTable: "CompanyAdmin",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FolderViews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FolderViewName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAdminId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolderViews_CompanyAdmin_CompanyAdminId",
                        column: x => x.CompanyAdminId,
                        principalTable: "CompanyAdmin",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MetaDataTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MetaDataTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MetaDataDataType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContainerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAdminId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaDataTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetaDataTypes_CompanyAdmin_CompanyAdminId",
                        column: x => x.CompanyAdminId,
                        principalTable: "CompanyAdmin",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserGroups",
                columns: table => new
                {
                    UserGroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserGroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyAdminId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroups", x => x.UserGroupId);
                    table.ForeignKey(
                        name: "FK_UserGroups_CompanyAdmin_CompanyAdminId",
                        column: x => x.CompanyAdminId,
                        principalTable: "CompanyAdmin",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyViewModels",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_CompanyViewModels_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId");
                    table.ForeignKey(
                        name: "FK_CompanyViewModels_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "ContainerMetaDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MetadataName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isRequired = table.Column<bool>(type: "bit", nullable: false),
                    ContainerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContainerTypeId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerMetaDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContainerMetaDatas_ContainerTypes_ContainerTypeId",
                        column: x => x.ContainerTypeId,
                        principalTable: "ContainerTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Values",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ValueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContainerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContainerTypeId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Values", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Values_ContainerTypes_ContainerTypeId",
                        column: x => x.ContainerTypeId,
                        principalTable: "ContainerTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OCR = table.Column<bool>(type: "bit", nullable: true),
                    VERSIONING = table.Column<bool>(type: "bit", nullable: true),
                    FileNameTypeId = table.Column<int>(type: "int", nullable: true),
                    FileNameTypeDocumentId = table.Column<int>(type: "int", nullable: true),
                    CompanyAdminId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_CompanyAdmin_CompanyAdminId",
                        column: x => x.CompanyAdminId,
                        principalTable: "CompanyAdmin",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Documents_FileNameTypeDocuments_FileNameTypeDocumentId",
                        column: x => x.FileNameTypeDocumentId,
                        principalTable: "FileNameTypeDocuments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FileNameMetaDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MetadataName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Seperator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileNameTypeDocumentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileNameMetaDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileNameMetaDatas_FileNameTypeDocuments_FileNameTypeDocumentId",
                        column: x => x.FileNameTypeDocumentId,
                        principalTable: "FileNameTypeDocuments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FolderViewLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SelectObject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ObjectType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Operator = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MetaDataList = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FolderViewId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderViewLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolderViewLists_FolderViews_FolderViewId",
                        column: x => x.FolderViewId,
                        principalTable: "FolderViews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContainerUserGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContainerTypeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContainerUserGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContainerUserGroups_ContainerTypes_ContainerTypeId",
                        column: x => x.ContainerTypeId,
                        principalTable: "ContainerTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ContainerUserGroups_UserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroups",
                        principalColumn: "UserGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentManagements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ViewDoc = table.Column<bool>(type: "bit", nullable: false),
                    DocSinAdd = table.Column<bool>(type: "bit", nullable: false),
                    DocMulAdd = table.Column<bool>(type: "bit", nullable: false),
                    DocCopy = table.Column<bool>(type: "bit", nullable: false),
                    DocMove = table.Column<bool>(type: "bit", nullable: false),
                    DocDelete = table.Column<bool>(type: "bit", nullable: false),
                    DocRename = table.Column<bool>(type: "bit", nullable: false),
                    DocPrivate = table.Column<bool>(type: "bit", nullable: false),
                    DocDown = table.Column<bool>(type: "bit", nullable: false),
                    DocPrint = table.Column<bool>(type: "bit", nullable: false),
                    ViewMatadata = table.Column<bool>(type: "bit", nullable: false),
                    EditMatadata = table.Column<bool>(type: "bit", nullable: false),
                    ShareDocInt = table.Column<bool>(type: "bit", nullable: false),
                    ShareDocExt = table.Column<bool>(type: "bit", nullable: false),
                    ShareSigExt = table.Column<bool>(type: "bit", nullable: false),
                    AuditLogDoc = table.Column<bool>(type: "bit", nullable: false),
                    DocVerView = table.Column<bool>(type: "bit", nullable: false),
                    DocRollBack = table.Column<bool>(type: "bit", nullable: false),
                    DownCsvRpt = table.Column<bool>(type: "bit", nullable: false),
                    AuditLogUser = table.Column<bool>(type: "bit", nullable: false),
                    AsgnDocUser = table.Column<bool>(type: "bit", nullable: false),
                    MaxDocUpSize = table.Column<bool>(type: "bit", nullable: false),
                    MaxDocUpNum = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserGroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentManagements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentManagements_UserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroups",
                        principalColumn: "UserGroupId");
                });

            migrationBuilder.CreateTable(
                name: "EndUserManagements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EditEmail = table.Column<bool>(type: "bit", nullable: false),
                    EditPassword = table.Column<bool>(type: "bit", nullable: false),
                    EditSign = table.Column<bool>(type: "bit", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserGroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndUserManagements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EndUserManagements_UserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroups",
                        principalColumn: "UserGroupId");
                });

            migrationBuilder.CreateTable(
                name: "MetaDataTypeUserGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MetaDataTypeId = table.Column<int>(type: "int", nullable: false),
                    UserGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaDataTypeUserGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetaDataTypeUserGroups_MetaDataTypes_MetaDataTypeId",
                        column: x => x.MetaDataTypeId,
                        principalTable: "MetaDataTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MetaDataTypeUserGroups_UserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroups",
                        principalColumn: "UserGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RADManagements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RadView = table.Column<bool>(type: "bit", nullable: false),
                    RadEdit = table.Column<bool>(type: "bit", nullable: false),
                    RadFormFill = table.Column<bool>(type: "bit", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserGroupId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RADManagements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RADManagements_UserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroups",
                        principalColumn: "UserGroupId");
                });

            migrationBuilder.CreateTable(
                name: "DocumentsUserGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentsId = table.Column<int>(type: "int", nullable: false),
                    UserGroupId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentsUserGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentsUserGroups_Documents_DocumentsId",
                        column: x => x.DocumentsId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentsUserGroups_UserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroups",
                        principalColumn: "UserGroupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MetaData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MetadataName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isRequired = table.Column<bool>(type: "bit", nullable: false),
                    DocumentsId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MetaData_Documents_DocumentsId",
                        column: x => x.DocumentsId,
                        principalTable: "Documents",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "MetaDataTypes",
                columns: new[] { "Id", "CompanyAdminId", "ContainerName", "MetaDataDataType", "MetaDataTypeName" },
                values: new object[,]
                {
                    { 1, null, null, "Text", "Document Create By" },
                    { 2, null, null, "Text", "Document Created Date" },
                    { 3, null, null, "Text", "Document Modified By" },
                    { 4, null, null, "Text", "Document Modified Date" },
                    { 5, null, null, "Text", "Workflow Started By User" },
                    { 6, null, null, "Text", "Workflow Used by Last User" },
                    { 7, null, null, "Text", "Workflow Viewed by Last User" },
                    { 8, null, null, "Text", "Current Date" },
                    { 9, null, null, "Text", "Current Time" },
                    { 10, null, null, "Text", "Current Date Time" },
                    { 11, null, null, "Text", "Metadata Create By" },
                    { 12, null, null, "Text", "Metadata Created Date" },
                    { 13, null, null, "Text", "Metadata Modified By" },
                    { 14, null, null, "Text", "Metadata Modified Date" },
                    { 15, null, null, "Text", "Container Create By" },
                    { 16, null, null, "Text", "Container Created Date" },
                    { 17, null, null, "Text", "Container Modified By" },
                    { 18, null, null, "Text", "Container Modified Date" }
                });

            migrationBuilder.InsertData(
                table: "PaymentCurrencies",
                columns: new[] { "PaymentCurrencyId", "PaymentCurrencyName" },
                values: new object[,]
                {
                    { 1, "Kes" },
                    { 2, "USD" }
                });

            migrationBuilder.InsertData(
                table: "PaymentIntervals",
                columns: new[] { "PaymentIntervalId", "PaymentIntervalName" },
                values: new object[,]
                {
                    { 1, "Monthly" },
                    { 2, "Quaterly" },
                    { 3, "Biannually" },
                    { 4, "Annually" }
                });

            migrationBuilder.InsertData(
                table: "PaymentTypes",
                columns: new[] { "PaymentTypeId", "PaymentTypeName" },
                values: new object[,]
                {
                    { 1, "Renewal" },
                    { 2, "One-Time" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAdmin_AspId",
                table: "CompanyAdmin",
                column: "AspId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyLogos_CompanyAdminId",
                table: "CompanyLogos",
                column: "CompanyAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyUsers_AspId",
                table: "CompanyUsers",
                column: "AspId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyViewModels_CompanyId",
                table: "CompanyViewModels",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyViewModels_UserId",
                table: "CompanyViewModels",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerMetaDatas_ContainerTypeId",
                table: "ContainerMetaDatas",
                column: "ContainerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerTypes_CompanyAdminId",
                table: "ContainerTypes",
                column: "CompanyAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerUserGroups_ContainerTypeId",
                table: "ContainerUserGroups",
                column: "ContainerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ContainerUserGroups_UserGroupId",
                table: "ContainerUserGroups",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentManagements_UserGroupId",
                table: "DocumentManagements",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CompanyAdminId",
                table: "Documents",
                column: "CompanyAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_FileNameTypeDocumentId",
                table: "Documents",
                column: "FileNameTypeDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsUserGroups_DocumentsId",
                table: "DocumentsUserGroups",
                column: "DocumentsId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentsUserGroups_UserGroupId",
                table: "DocumentsUserGroups",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTypeMetadata_CompanyAdminId",
                table: "DocumentTypeMetadata",
                column: "CompanyAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_EndUserManagements_UserGroupId",
                table: "EndUserManagements",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_FileNameMetaDatas_FileNameTypeDocumentId",
                table: "FileNameMetaDatas",
                column: "FileNameTypeDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_FileNameTypeDocuments_CompanyAdminId",
                table: "FileNameTypeDocuments",
                column: "CompanyAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderViewLists_FolderViewId",
                table: "FolderViewLists",
                column: "FolderViewId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderViews_CompanyAdminId",
                table: "FolderViews",
                column: "CompanyAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_MetaData_DocumentsId",
                table: "MetaData",
                column: "DocumentsId");

            migrationBuilder.CreateIndex(
                name: "IX_MetaDataTypes_CompanyAdminId",
                table: "MetaDataTypes",
                column: "CompanyAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_MetaDataTypeUserGroups_MetaDataTypeId",
                table: "MetaDataTypeUserGroups",
                column: "MetaDataTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MetaDataTypeUserGroups_UserGroupId",
                table: "MetaDataTypeUserGroups",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_AspId",
                table: "Profiles",
                column: "AspId");

            migrationBuilder.CreateIndex(
                name: "IX_RADManagements_UserGroupId",
                table: "RADManagements",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SmgCosts_SmgViewModelId",
                table: "SmgCosts",
                column: "SmgViewModelId");

            migrationBuilder.CreateIndex(
                name: "IX_SmgInfos_PaymentCurrencyId",
                table: "SmgInfos",
                column: "PaymentCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_SmgInfos_PaymentIntervalId",
                table: "SmgInfos",
                column: "PaymentIntervalId");

            migrationBuilder.CreateIndex(
                name: "IX_SmgInfos_PaymentTypeId",
                table: "SmgInfos",
                column: "PaymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserGroups_CompanyAdminId",
                table: "UserGroups",
                column: "CompanyAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CompanyId",
                table: "Users",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Values_ContainerTypeId",
                table: "Values",
                column: "ContainerTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "CompanyLogos");

            migrationBuilder.DropTable(
                name: "CompanyUsers");

            migrationBuilder.DropTable(
                name: "CompanyViewModels");

            migrationBuilder.DropTable(
                name: "ContainerMetaDatas");

            migrationBuilder.DropTable(
                name: "ContainerUserGroups");

            migrationBuilder.DropTable(
                name: "DocumentManagements");

            migrationBuilder.DropTable(
                name: "DocumentsUserGroups");

            migrationBuilder.DropTable(
                name: "DocumentTypeMetadata");

            migrationBuilder.DropTable(
                name: "EndUserManagements");

            migrationBuilder.DropTable(
                name: "FileNameMetaDatas");

            migrationBuilder.DropTable(
                name: "FolderViewLists");

            migrationBuilder.DropTable(
                name: "MetaData");

            migrationBuilder.DropTable(
                name: "MetaDataTypeUserGroups");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropTable(
                name: "RADManagements");

            migrationBuilder.DropTable(
                name: "SelectListGroup");

            migrationBuilder.DropTable(
                name: "SmgCosts");

            migrationBuilder.DropTable(
                name: "SmgInfos");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "Values");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "FolderViews");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "MetaDataTypes");

            migrationBuilder.DropTable(
                name: "UserGroups");

            migrationBuilder.DropTable(
                name: "SmgViewModels");

            migrationBuilder.DropTable(
                name: "PaymentCurrencies");

            migrationBuilder.DropTable(
                name: "PaymentIntervals");

            migrationBuilder.DropTable(
                name: "PaymentTypes");

            migrationBuilder.DropTable(
                name: "ContainerTypes");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "FileNameTypeDocuments");

            migrationBuilder.DropTable(
                name: "CompanyAdmin");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
