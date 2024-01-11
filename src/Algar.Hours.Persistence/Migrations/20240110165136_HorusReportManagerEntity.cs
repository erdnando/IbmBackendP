using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class HorusReportManagerEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HorusReportManagerEntity",
                columns: table => new
                {
                    IdHorusReportManager = table.Column<Guid>(type: "uuid", nullable: false),
                    UserEntityManagerId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartTime = table.Column<string>(type: "text", nullable: false),
                    EndTime = table.Column<string>(type: "text", nullable: false),
                    TypeReport = table.Column<string>(type: "text", nullable: false),
                    CountHours = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Observations = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorusReportManagerEntity", x => x.IdHorusReportManager);
                    table.ForeignKey(
                        name: "FK_HorusReportManagerEntity_UserEntity_UserEntityId",
                        column: x => x.UserEntityId,
                        principalTable: "UserEntity",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HorusReportManagerEntity_UserEntityId",
                table: "HorusReportManagerEntity",
                column: "UserEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HorusReportManagerEntity");
        }
    }
}
