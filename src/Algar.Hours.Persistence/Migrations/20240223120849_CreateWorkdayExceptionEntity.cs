using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateWorkdayExceptionEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkdayExceptionEntity",
                columns: table => new
                {
                    IdWorkdayException = table.Column<Guid>(type: "uuid", nullable: false),
                    UserEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeCode = table.Column<string>(type: "text", nullable: false),
                    EmployeeName = table.Column<string>(type: "text", nullable: false),
                    CountryEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OriginalStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    OriginalEndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    RealDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RealStartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    RealEndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ReportType = table.Column<string>(type: "text", nullable: false),
                    Justification = table.Column<string>(type: "text", nullable: false),
                    AprrovingManager = table.Column<string>(type: "text", nullable: false),

                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkdayExceptionEntity", x => x.IdWorkdayException);
                    table.ForeignKey(
                        name: "FK_WorkdayExceptionEntity_UserEntity_UserEntityId",
                        column: x => x.UserEntityId,
                        principalTable: "UserEntity",
                        principalColumn: "IdUser",
                        onUpdate: ReferentialAction.Cascade, 
                        onDelete: ReferentialAction.NoAction
                    );
                    table.ForeignKey(
                        name: "FK_WorkdayExceptionEntity_UserEntity_CountryEntityId",
                        column: x => x.CountryEntityId,
                        principalTable: "CountryEntity",
                        principalColumn: "IdCountry",
                        onUpdate: ReferentialAction.Cascade,
                        onDelete: ReferentialAction.NoAction
                    );
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkdayExceptionEntity");
        }
    }
}
