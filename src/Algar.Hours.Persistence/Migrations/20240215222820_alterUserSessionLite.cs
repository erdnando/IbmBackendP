using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class alterUserSessionLite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSessionEntity_RoleEntity_RoleEntityId",
                table: "UserSessionEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_UserSessionEntity_UserEntity_UserEntityId",
                table: "UserSessionEntity");

            migrationBuilder.DropIndex(
                name: "IX_UserSessionEntity_RoleEntityId",
                table: "UserSessionEntity");

            migrationBuilder.DropIndex(
                name: "IX_UserSessionEntity_UserEntityId",
                table: "UserSessionEntity");

            migrationBuilder.DropColumn(
                name: "RoleEntityId",
                table: "UserSessionEntity");

            migrationBuilder.DropColumn(
                name: "UserEntityId",
                table: "UserSessionEntity");

            migrationBuilder.DropColumn(
                name: "eventAlias",
                table: "UserSessionEntity");

            migrationBuilder.DropColumn(
                name: "resultOperation",
                table: "UserSessionEntity");

            migrationBuilder.RenameColumn(
                name: "tag",
                table: "UserSessionEntity",
                newName: "sUserEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "sUserEntityId",
                table: "UserSessionEntity",
                newName: "tag");

            migrationBuilder.AddColumn<Guid>(
                name: "RoleEntityId",
                table: "UserSessionEntity",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserEntityId",
                table: "UserSessionEntity",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "eventAlias",
                table: "UserSessionEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "resultOperation",
                table: "UserSessionEntity",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessionEntity_RoleEntityId",
                table: "UserSessionEntity",
                column: "RoleEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessionEntity_UserEntityId",
                table: "UserSessionEntity",
                column: "UserEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessionEntity_RoleEntity_RoleEntityId",
                table: "UserSessionEntity",
                column: "RoleEntityId",
                principalTable: "RoleEntity",
                principalColumn: "IdRole",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserSessionEntity_UserEntity_UserEntityId",
                table: "UserSessionEntity",
                column: "UserEntityId",
                principalTable: "UserEntity",
                principalColumn: "IdUser",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
