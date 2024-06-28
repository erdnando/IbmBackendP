using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class permisosTLSDEV : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ParametersTseInitialEntity_IdCarga",
                table: "ParametersTseInitialEntity",
                column: "IdCarga");

            migrationBuilder.CreateIndex(
                name: "IX_ParametersSteInitialEntity_IdCarga",
                table: "ParametersSteInitialEntity",
                column: "IdCarga");

            migrationBuilder.CreateIndex(
                name: "IX_ParametersArpInitialEntity_IdCarga",
                table: "ParametersArpInitialEntity",
                column: "IdCarga");

            migrationBuilder.AddForeignKey(
                name: "FK_ParametersArpInitialEntity_ARPLoadEntity_IdCarga",
                table: "ParametersArpInitialEntity",
                column: "IdCarga",
                principalTable: "ARPLoadEntity",
                principalColumn: "IdArpLoad");

            migrationBuilder.AddForeignKey(
                name: "FK_ParametersSteInitialEntity_ARPLoadEntity_IdCarga",
                table: "ParametersSteInitialEntity",
                column: "IdCarga",
                principalTable: "ARPLoadEntity",
                principalColumn: "IdArpLoad");

            migrationBuilder.AddForeignKey(
                name: "FK_ParametersTseInitialEntity_ARPLoadEntity_IdCarga",
                table: "ParametersTseInitialEntity",
                column: "IdCarga",
                principalTable: "ARPLoadEntity",
                principalColumn: "IdArpLoad");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParametersArpInitialEntity_ARPLoadEntity_IdCarga",
                table: "ParametersArpInitialEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ParametersSteInitialEntity_ARPLoadEntity_IdCarga",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ParametersTseInitialEntity_ARPLoadEntity_IdCarga",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropIndex(
                name: "IX_ParametersTseInitialEntity_IdCarga",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropIndex(
                name: "IX_ParametersSteInitialEntity_IdCarga",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropIndex(
                name: "IX_ParametersArpInitialEntity_IdCarga",
                table: "ParametersArpInitialEntity");
        }
    }
}
