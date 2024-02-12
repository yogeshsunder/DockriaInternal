using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class smg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "SmgCosts");

            migrationBuilder.DropColumn(
                name: "Vat",
                table: "SmgCosts");

            migrationBuilder.DropColumn(
                name: "VatPercentage",
                table: "SmgCosts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                table: "SmgCosts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Vat",
                table: "SmgCosts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VatPercentage",
                table: "SmgCosts",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
