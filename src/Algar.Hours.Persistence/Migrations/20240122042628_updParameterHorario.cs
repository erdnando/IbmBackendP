using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updParameterHorario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HoraFinHorario",
                table: "ParametersTseInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HoraInicioHoraio",
                table: "ParametersTseInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HoraFinHorario",
                table: "ParametersSteInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HoraInicioHoraio",
                table: "ParametersSteInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HoraFinHorario",
                table: "ParametersArpInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HoraInicioHoraio",
                table: "ParametersArpInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoraFinHorario",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropColumn(
                name: "HoraInicioHoraio",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropColumn(
                name: "HoraFinHorario",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropColumn(
                name: "HoraInicioHoraio",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropColumn(
                name: "HoraFinHorario",
                table: "ParametersArpInitialEntity");

            migrationBuilder.DropColumn(
                name: "HoraInicioHoraio",
                table: "ParametersArpInitialEntity");
        }
    }
}
