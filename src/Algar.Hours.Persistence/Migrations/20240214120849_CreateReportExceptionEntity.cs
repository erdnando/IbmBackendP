using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateReportExceptionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportExceptionEntity",
                columns: table => new
                {
                    IdReportException = table.Column<Guid>(type: "uuid", nullable: false),
                    UserEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Report = table.Column<string>(type: "text", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExceptionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ExceptionUserEntityId = table.Column<Guid>(type: "uuid", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportExceptionEntity", x => x.IdReportException);
                    table.ForeignKey(
                        name: "FK_ReportExceptionEntity_UserEntity_UserEntityId",
                        column: x => x.UserEntityId,
                        principalTable: "UserEntity",
                        principalColumn: "IdUser",
                        onUpdate: ReferentialAction.Cascade, 
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "FK_ReportExceptionEntity_UserEntity_ExceptionUserEntityId",
                        column: x => x.ExceptionUserEntityId,
                        principalTable: "UserEntity",
                        principalColumn: "IdUser",
                        onUpdate: ReferentialAction.Cascade,
                        onDelete: ReferentialAction.SetNull
                    );
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportExceptionEntity");
        }
    }
}
