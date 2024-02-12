using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class addDocumentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OCR = table.Column<bool>(type: "bit", nullable: false),
                    VERSIONING = table.Column<bool>(type: "bit", nullable: false),
                    AutoFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserGroupId = table.Column<int>(type: "int", nullable: true),
                    CompanyAdminId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_CompanyAdmin_CompanyAdminId",
                        column: x => x.CompanyAdminId,
                        principalTable: "CompanyAdmin",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Documents_UserGroups_UserGroupId",
                        column: x => x.UserGroupId,
                        principalTable: "UserGroups",
                        principalColumn: "UserGroupId");
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

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CompanyAdminId",
                table: "Documents",
                column: "CompanyAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UserGroupId",
                table: "Documents",
                column: "UserGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MetaData_DocumentsId",
                table: "MetaData",
                column: "DocumentsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetaData");

            migrationBuilder.DropTable(
                name: "Documents");
        }
    }
}
