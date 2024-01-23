using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCargaMetricFields2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ARPXProceso",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "STEXProceso",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TSEXProceso",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ARPXProceso",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "STEXProceso",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "TSEXProceso",
                table: "ARPLoadEntity");
        }
    }
}
