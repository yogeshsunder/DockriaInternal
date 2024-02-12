using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class addCompanyLogoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_CompanyLogos_CompanyAdminId",
                table: "CompanyLogos",
                column: "CompanyAdminId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyLogos");
        }
    }
}
