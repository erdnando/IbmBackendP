using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class removeExtraFieldshorario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HorarioExistenteFin",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropColumn(
                name: "HorarioExistenteInicio",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropColumn(
                name: "HorarioExistenteFin",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropColumn(
                name: "HorarioExistenteInicio",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropColumn(
                name: "HorarioExistenteFin",
                table: "ParametersArpInitialEntity");

            migrationBuilder.DropColumn(
                name: "HorarioExistenteInicio",
                table: "ParametersArpInitialEntity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HorarioExistenteFin",
                table: "ParametersTseInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HorarioExistenteInicio",
                table: "ParametersTseInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HorarioExistenteFin",
                table: "ParametersSteInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HorarioExistenteInicio",
                table: "ParametersSteInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HorarioExistenteFin",
                table: "ParametersArpInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HorarioExistenteInicio",
                table: "ParametersArpInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
