using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addEstatusOrigenPortalDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EstatusOrigen",
                table: "ParametersTseInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EstatusOrigen",
                table: "ParametersSteInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EstatusOrigen",
                table: "ParametersArpInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EstatusOrigen",
                table: "HorusReportEntity",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstatusOrigen",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropColumn(
                name: "EstatusOrigen",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropColumn(
                name: "EstatusOrigen",
                table: "ParametersArpInitialEntity");

            migrationBuilder.DropColumn(
                name: "EstatusOrigen",
                table: "HorusReportEntity");
        }
    }
}
