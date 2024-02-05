using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addingMetics2Duplicated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ARPOmitidosXDuplicidad",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "STEOmitidosXDuplicidad",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TSEOmitidosXDuplicidad",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ARPOmitidosXDuplicidad",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "STEOmitidosXDuplicidad",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "TSEOmitidosXDuplicidad",
                table: "ARPLoadEntity");
        }
    }
}
