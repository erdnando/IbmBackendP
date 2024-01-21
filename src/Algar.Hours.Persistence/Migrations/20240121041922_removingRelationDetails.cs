using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class removingRelationDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParametersArpInitialEntity_ARPLoadDetailEntity_ARPLoadDetai~",
                table: "ParametersArpInitialEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ParametersSteInitialEntity_STELoadEntity_STELoadEntityId",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_ParametersTseInitialEntity_TSELoadEntity_TSELoadEntityIdTSE~",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropIndex(
                name: "IX_ParametersTseInitialEntity_TSELoadEntityIdTSELoad",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropIndex(
                name: "IX_ParametersSteInitialEntity_STELoadEntityId",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropIndex(
                name: "IX_ParametersArpInitialEntity_ARPLoadDetailEntityId",
                table: "ParametersArpInitialEntity");

            migrationBuilder.DropColumn(
                name: "TSELoadEntityIdTSELoad",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropColumn(
                name: "STELoadEntityId",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropColumn(
                name: "ARPLoadDetailEntityId",
                table: "ParametersArpInitialEntity");

            migrationBuilder.AddColumn<string>(
                name: "TOTALHORAS",
                table: "ARPLoadDetailEntity",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TOTALHORAS",
                table: "ARPLoadDetailEntity");

            migrationBuilder.AddColumn<Guid>(
                name: "TSELoadEntityIdTSELoad",
                table: "ParametersTseInitialEntity",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "STELoadEntityId",
                table: "ParametersSteInitialEntity",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ARPLoadDetailEntityId",
                table: "ParametersArpInitialEntity",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ParametersTseInitialEntity_TSELoadEntityIdTSELoad",
                table: "ParametersTseInitialEntity",
                column: "TSELoadEntityIdTSELoad");

            migrationBuilder.CreateIndex(
                name: "IX_ParametersSteInitialEntity_STELoadEntityId",
                table: "ParametersSteInitialEntity",
                column: "STELoadEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ParametersArpInitialEntity_ARPLoadDetailEntityId",
                table: "ParametersArpInitialEntity",
                column: "ARPLoadDetailEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParametersArpInitialEntity_ARPLoadDetailEntity_ARPLoadDetai~",
                table: "ParametersArpInitialEntity",
                column: "ARPLoadDetailEntityId",
                principalTable: "ARPLoadDetailEntity",
                principalColumn: "IdDetail",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParametersSteInitialEntity_STELoadEntity_STELoadEntityId",
                table: "ParametersSteInitialEntity",
                column: "STELoadEntityId",
                principalTable: "STELoadEntity",
                principalColumn: "IdSTELoad",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ParametersTseInitialEntity_TSELoadEntity_TSELoadEntityIdTSE~",
                table: "ParametersTseInitialEntity",
                column: "TSELoadEntityIdTSELoad",
                principalTable: "TSELoadEntity",
                principalColumn: "IdTSELoad",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
