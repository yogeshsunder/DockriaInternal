using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class updateForiegnkey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_CompanyAdmin_Id",
                table: "UserGroups");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserGroups",
                newName: "CompanyAdminId");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroups_Id",
                table: "UserGroups",
                newName: "IX_UserGroups_CompanyAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_CompanyAdmin_CompanyAdminId",
                table: "UserGroups",
                column: "CompanyAdminId",
                principalTable: "CompanyAdmin",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGroups_CompanyAdmin_CompanyAdminId",
                table: "UserGroups");

            migrationBuilder.RenameColumn(
                name: "CompanyAdminId",
                table: "UserGroups",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_UserGroups_CompanyAdminId",
                table: "UserGroups",
                newName: "IX_UserGroups_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGroups_CompanyAdmin_Id",
                table: "UserGroups",
                column: "Id",
                principalTable: "CompanyAdmin",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
