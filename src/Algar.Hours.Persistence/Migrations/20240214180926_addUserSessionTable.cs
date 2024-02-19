using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addUserSessionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSessionEntity",
                columns: table => new
                {
                    IdSession = table.Column<Guid>(type: "uuid", nullable: false),
                    UserEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    LogDateEvent = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    eventAlias = table.Column<string>(type: "text", nullable: false),
                    parameters = table.Column<string>(type: "text", nullable: false),
                    operation = table.Column<string>(type: "text", nullable: false),
                    resultOperation = table.Column<string>(type: "text", nullable: false),
                    RoleEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    tag = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessionEntity", x => x.IdSession);
                    table.ForeignKey(
                        name: "FK_UserSessionEntity_RoleEntity_RoleEntityId",
                        column: x => x.RoleEntityId,
                        principalTable: "RoleEntity",
                        principalColumn: "IdRole",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSessionEntity_UserEntity_UserEntityId",
                        column: x => x.UserEntityId,
                        principalTable: "UserEntity",
                        principalColumn: "IdUser",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSessionEntity_RoleEntityId",
                table: "UserSessionEntity",
                column: "RoleEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessionEntity_UserEntityId",
                table: "UserSessionEntity",
                column: "UserEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSessionEntity");
        }
    }
}
