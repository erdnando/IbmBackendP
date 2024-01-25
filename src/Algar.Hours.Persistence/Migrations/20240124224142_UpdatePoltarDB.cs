﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePoltarDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PortalDBAproveHistoryEntity",
                table: "PortalDBAproveHistoryEntity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PortalDBAproveHistoryEntity",
                table: "PortalDBAproveHistoryEntity",
                column: "IdPortalDB");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PortalDBAproveHistoryEntity",
                table: "PortalDBAproveHistoryEntity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PortalDBAproveHistoryEntity",
                table: "PortalDBAproveHistoryEntity",
                column: "IdPortalDBAproveHistory");
        }
    }
}
