using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ReportTypeUsersExceptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<DateTimeOffset>(
            //    name: "CreationDate",
            //    table: "WorkdayExceptionEntity",
            //    type: "timestamp with time zone",
            //    nullable: false,
            //    defaultValueSql: "CURRENT_TIMESTAMP",
            //    oldClrType: typeof(DateTime),
            //    oldType: "timestamp with time zone");

            //migrationBuilder.AlterColumn<bool>(
            //    name: "Active",
            //    table: "WorkdayExceptionEntity",
            //    type: "boolean",
            //    nullable: false,
            //    defaultValue: true,
            //    oldClrType: typeof(bool),
            //    oldType: "boolean");

            migrationBuilder.AddColumn<string>(
                name: "ReportType",
                table: "UsersExceptions",
                type: "text",
                nullable: false,
                defaultValue: "");

            //migrationBuilder.AddColumn<string>(
            //    name: "Acciones",
            //    table: "ParametersTseInitialEntity",
            //    type: "text",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Actividad",
            //    table: "ParametersTseInitialEntity",
            //    type: "text",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Problemas",
            //    table: "ParametersTseInitialEntity",
            //    type: "text",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Acciones",
            //    table: "ParametersSteInitialEntity",
            //    type: "text",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Actividad",
            //    table: "ParametersSteInitialEntity",
            //    type: "text",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Problemas",
            //    table: "ParametersSteInitialEntity",
            //    type: "text",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Acciones",
            //    table: "ParametersArpInitialEntity",
            //    type: "text",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Actividad",
            //    table: "ParametersArpInitialEntity",
            //    type: "text",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Problemas",
            //    table: "ParametersArpInitialEntity",
            //    type: "text",
            //    nullable: true);

            //migrationBuilder.CreateTable(
            //    name: "TemplateEntity",
            //    columns: table => new
            //    {
            //        IdTemplate = table.Column<Guid>(type: "uuid", nullable: false),
            //        Name = table.Column<string>(type: "text", nullable: false),
            //        Description = table.Column<string>(type: "text", nullable: false),
            //        FileName = table.Column<string>(type: "text", nullable: false),
            //        FileContentType = table.Column<string>(type: "text", nullable: false),
            //        FileData = table.Column<byte[]>(type: "bytea", nullable: false),
            //        CreationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TemplateEntity", x => x.IdTemplate);
            //    });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            

            migrationBuilder.DropColumn(
                name: "ReportType",
                table: "UsersExceptions");

            //migrationBuilder.DropColumn(
            //    name: "Acciones",
            //    table: "ParametersTseInitialEntity");

            //migrationBuilder.DropColumn(
            //    name: "Actividad",
            //    table: "ParametersTseInitialEntity");

            //migrationBuilder.DropColumn(
            //    name: "Problemas",
            //    table: "ParametersTseInitialEntity");

            //migrationBuilder.DropColumn(
            //    name: "Acciones",
            //    table: "ParametersSteInitialEntity");

            //migrationBuilder.DropColumn(
            //    name: "Actividad",
            //    table: "ParametersSteInitialEntity");

            //migrationBuilder.DropColumn(
            //    name: "Problemas",
            //    table: "ParametersSteInitialEntity");

            //migrationBuilder.DropColumn(
            //    name: "Acciones",
            //    table: "ParametersArpInitialEntity");

            //migrationBuilder.DropColumn(
            //    name: "Actividad",
            //    table: "ParametersArpInitialEntity");

            //migrationBuilder.DropColumn(
            //    name: "Problemas",
            //    table: "ParametersArpInitialEntity");

            //migrationBuilder.AlterColumn<DateTime>(
            //    name: "CreationDate",
            //    table: "WorkdayExceptionEntity",
            //    type: "timestamp with time zone",
            //    nullable: false,
            //    oldClrType: typeof(DateTimeOffset),
            //    oldType: "timestamp with time zone",
            //    oldDefaultValueSql: "CURRENT_TIMESTAMP");

            //migrationBuilder.AlterColumn<bool>(
            //    name: "Active",
            //    table: "WorkdayExceptionEntity",
            //    type: "boolean",
            //    nullable: false,
            //    oldClrType: typeof(bool),
            //    oldType: "boolean",
            //    oldDefaultValue: true);
        }
    }
}
