using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PortalDbNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PortalDBEntity",
                columns: table => new
                {
                    IdPortalDb = table.Column<Guid>(type: "uuid", nullable: false),
                    UserEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartTime = table.Column<string>(type: "text", nullable: false),
                    EndTime = table.Column<string>(type: "text", nullable: false),
                    ClientEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TipoReporte = table.Column<int>(type: "integer", nullable: false),
                    Acitivity = table.Column<int>(type: "integer", nullable: false),
                    CountHours = table.Column<string>(type: "text", nullable: false),
                    NumberReport = table.Column<int>(type: "integer", nullable: false),
                    ApproverId = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortalDBEntity", x => x.IdPortalDb);
                    table.ForeignKey(
                        name: "FK_PortalDBEntity_ClientEntity_ClientEntityId",
                        column: x => x.ClientEntityId,
                        principalTable: "ClientEntity",
                        principalColumn: "IdClient");
                    table.ForeignKey(
                        name: "FK_PortalDBEntity_UserEntity_UserEntityId",
                        column: x => x.UserEntityId,
                        principalTable: "UserEntity",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortalDBAproveHistoryEntity",
                columns: table => new
                {
                    IdPortalDBAproveHistory = table.Column<Guid>(type: "uuid", nullable: false),
                    IdPortalDB = table.Column<Guid>(type: "uuid", nullable: false),
                    PortalDBEntityIdPortalDb = table.Column<Guid>(type: "uuid", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    DateApprovalCancellation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApproverId = table.Column<string>(type: "text", nullable: false),
                    TipoReporte = table.Column<int>(type: "integer", nullable: false),
                    Acitivity = table.Column<int>(type: "integer", nullable: false),
                    NumberReport = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortalDBAproveHistoryEntity", x => x.IdPortalDBAproveHistory);
                    table.ForeignKey(
                        name: "FK_PortalDBAproveHistoryEntity_PortalDBEntity_PortalDBEntityId~",
                        column: x => x.PortalDBEntityIdPortalDb,
                        principalTable: "PortalDBEntity",
                        principalColumn: "IdPortalDb",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortalDBAproveHistoryEntity_PortalDBEntityIdPortalDb",
                table: "PortalDBAproveHistoryEntity",
                column: "PortalDBEntityIdPortalDb");

            migrationBuilder.CreateIndex(
                name: "IX_PortalDBEntity_ClientEntityId",
                table: "PortalDBEntity",
                column: "ClientEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_PortalDBEntity_UserEntityId",
                table: "PortalDBEntity",
                column: "UserEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortalDBAproveHistoryEntity");

            migrationBuilder.DropTable(
                name: "PortalDBEntity");
        }
    }
}
