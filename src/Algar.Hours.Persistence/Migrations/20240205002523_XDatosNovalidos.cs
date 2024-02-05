using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class XDatosNovalidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ARPXDatosNovalidos",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "STEXDatosNovalidos",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TSEXDatosNovalidos",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ARPXDatosNovalidos",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "STEXDatosNovalidos",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "TSEXDatosNovalidos",
                table: "ARPLoadEntity");
        }
    }
}
