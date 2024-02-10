using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class alterHorusAndAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApproverId",
                table: "HorusReportEntity");

            migrationBuilder.DropColumn(
                name: "ApproverId2",
                table: "HorusReportEntity");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "HorusReportEntity");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "HorusReportEntity");

            migrationBuilder.DropColumn(
                name: "DateApprovalCancellation",
                table: "assignmentReports");

            migrationBuilder.DropColumn(
                name: "Employee",
                table: "assignmentReports");

            migrationBuilder.DropColumn(
                name: "StrReport",
                table: "assignmentReports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApproverId",
                table: "HorusReportEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ApproverId2",
                table: "HorusReportEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "HorusReportEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "HorusReportEntity",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateApprovalCancellation",
                table: "assignmentReports",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Employee",
                table: "assignmentReports",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StrReport",
                table: "assignmentReports",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
