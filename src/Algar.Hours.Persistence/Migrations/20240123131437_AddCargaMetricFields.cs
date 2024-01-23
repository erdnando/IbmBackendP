using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCargaMetricFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ARPCarga",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ARPOmitidos",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ARPXHorario",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ARPXOverlaping",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ARPXOvertime",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "STECarga",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "STEEXOvertime",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "STEOmitidos",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "STEXHorario",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "STEXOverlaping",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TSECarga",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TSEOmitidos",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TSEXHorario",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TSEXOverlaping",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TSEXOvertime",
                table: "ARPLoadEntity",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ARPCarga",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "ARPOmitidos",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "ARPXHorario",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "ARPXOverlaping",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "ARPXOvertime",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "STECarga",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "STEEXOvertime",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "STEOmitidos",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "STEXHorario",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "STEXOverlaping",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "TSECarga",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "TSEOmitidos",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "TSEXHorario",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "TSEXOverlaping",
                table: "ARPLoadEntity");

            migrationBuilder.DropColumn(
                name: "TSEXOvertime",
                table: "ARPLoadEntity");
        }
    }
}
