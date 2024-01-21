using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateParametersEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmployeeCode",
                table: "ParametersTseInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeCode",
                table: "ParametersSteInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EmployeeCode",
                table: "ParametersArpInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeCode",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropColumn(
                name: "EmployeeCode",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropColumn(
                name: "EmployeeCode",
                table: "ParametersArpInitialEntity");
        }
    }
}
