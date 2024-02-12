using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class updateProfileTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AspId",
                table: "Profiles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_AspId",
                table: "Profiles",
                column: "AspId");

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_AspNetUsers_AspId",
                table: "Profiles",
                column: "AspId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_AspNetUsers_AspId",
                table: "Profiles");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_AspId",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "AspId",
                table: "Profiles");
        }
    }
}
