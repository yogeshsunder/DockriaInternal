using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class sessionss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginDateTime",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "LogoutDateTime",
                table: "UserSessions");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "UserSessions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LoginDateTime",
                table: "UserSessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LogoutDateTime",
                table: "UserSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "UserSessions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
