using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addingAuxfieldsDashboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FECHA_REP",
                table: "ParametersTseInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TOTAL_MINUTOS",
                table: "ParametersTseInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "totalHoras",
                table: "ParametersTseInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FECHA_REP",
                table: "ParametersSteInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TOTAL_MINUTOS",
                table: "ParametersSteInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "totalHoras",
                table: "ParametersSteInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FECHA_REP",
                table: "ParametersArpInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TOTAL_MINUTOS",
                table: "ParametersArpInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "totalHoras",
                table: "ParametersArpInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FECHA_REP",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropColumn(
                name: "TOTAL_MINUTOS",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropColumn(
                name: "totalHoras",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropColumn(
                name: "FECHA_REP",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropColumn(
                name: "TOTAL_MINUTOS",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropColumn(
                name: "totalHoras",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropColumn(
                name: "FECHA_REP",
                table: "ParametersArpInitialEntity");

            migrationBuilder.DropColumn(
                name: "TOTAL_MINUTOS",
                table: "ParametersArpInitialEntity");

            migrationBuilder.DropColumn(
                name: "totalHoras",
                table: "ParametersArpInitialEntity");
        }
    }
}
