using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class horarioYRelacionCarga : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<Guid>(
                name: "IdCarga",
                table: "ParametersTseInitialEntity",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.AddColumn<Guid>(
                name: "IdCarga",
                table: "ParametersSteInitialEntity",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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

            migrationBuilder.AddColumn<Guid>(
                name: "IdCarga",
                table: "ParametersArpInitialEntity",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HorarioExistenteFin",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropColumn(
                name: "HorarioExistenteInicio",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropColumn(
                name: "IdCarga",
                table: "ParametersTseInitialEntity");

            migrationBuilder.DropColumn(
                name: "HorarioExistenteFin",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropColumn(
                name: "HorarioExistenteInicio",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropColumn(
                name: "IdCarga",
                table: "ParametersSteInitialEntity");

            migrationBuilder.DropColumn(
                name: "HorarioExistenteFin",
                table: "ParametersArpInitialEntity");

            migrationBuilder.DropColumn(
                name: "HorarioExistenteInicio",
                table: "ParametersArpInitialEntity");

            migrationBuilder.DropColumn(
                name: "IdCarga",
                table: "ParametersArpInitialEntity");
        }
    }
}
