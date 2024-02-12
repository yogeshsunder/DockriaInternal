using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class iconn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoData1",
                table: "CompanyLogos");

            migrationBuilder.DropColumn(
                name: "LogoData2",
                table: "CompanyLogos");

            migrationBuilder.AddColumn<byte[]>(
                name: "IconData",
                table: "CompanyLogos",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconData",
                table: "CompanyLogos");

            migrationBuilder.AddColumn<byte[]>(
                name: "LogoData1",
                table: "CompanyLogos",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "LogoData2",
                table: "CompanyLogos",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
