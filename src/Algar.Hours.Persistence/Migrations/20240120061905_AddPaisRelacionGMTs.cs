using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPaisRelacionGMTs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaisRelacionGMTEntity",
                columns: table => new
                {
                    IdPaisRelacionGMTEntity = table.Column<Guid>(type: "uuid", nullable: false),
                    NameCountrySelected = table.Column<string>(type: "text", nullable: false),
                    NameCountryCompare = table.Column<string>(type: "text", nullable: false),
                    TimeDifference = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaisRelacionGMTEntity", x => x.IdPaisRelacionGMTEntity);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaisRelacionGMTEntity");
        }
    }
}
