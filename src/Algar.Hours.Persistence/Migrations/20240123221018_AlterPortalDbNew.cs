using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Algar.Hours.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AlterPortalDbNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"PortalDBEntity\"  OWNER to \"ibm-cloud-base-user\";");
            migrationBuilder.Sql("ALTER TABLE IF EXISTS portal_tls.\"PortalDBAproveHistoryEntity\"  OWNER to \"ibm-cloud-base-user\";");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
