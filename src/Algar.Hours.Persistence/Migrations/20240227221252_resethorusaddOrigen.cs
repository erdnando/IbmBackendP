using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class resethorusaddOrigen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Origen",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropColumn(
                name: "Origen",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropColumn(
                name: "Origen",
                table: "ParametersArpInitialEntity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Origen",
                table: "ParametersTseInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Origen",
                table: "ParametersSteInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Origen",
                table: "ParametersArpInitialEntity",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
